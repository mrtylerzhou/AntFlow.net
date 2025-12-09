using System.Text.Json;
using antflowcore.bpmn.listener;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using AntFlowCore.Vo;

namespace antflowcore.bpmn.service;

public class RuntimeService
{
     private readonly AfTaskInstService _taskInstService;
     private readonly AFTaskService _afTaskService;
     private readonly TaskService _taskService;
     private readonly AfTaskInstService _afTaskInstService;
     private readonly AFDeploymentService _afDeploymentService;
     private readonly ITaskListener _taskListener;
     private readonly AFExecutionService _executionService;

     public RuntimeService(
          AfTaskInstService taskInstService, 
          AFTaskService afTaskService,
          TaskService taskService,
          AfTaskInstService afTaskInstService,
          AFExecutionService afExecutionService,
          AFDeploymentService afDeploymentService,
          ITaskListener taskListener, 
          AFExecutionService executionService)
     {
          _taskInstService = taskInstService; 
          _afTaskService = afTaskService;
          _taskService = taskService;
          _afTaskInstService = afTaskInstService;
          _afDeploymentService = afDeploymentService;
          _taskListener = taskListener;
          _executionService = executionService;
     }

     public ExecutionEntity StartProcessInstance(BpmnConfCommonVo bpmnConfCommonVo,
          BpmnStartConditionsVo bpmnStartConditions, string deploymentId)
     {
          List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = bpmnConfCommonVo.ElementList;
          string procInstId = StrongUuidGenerator.GetNextId();
         
          List<BpmnConfCommonElementVo> firstAssigneeNodes =
               BpmnFlowUtil.GetFirstAssigneeNodes(bpmnConfCommonElementVos);
          IDictionary<string, IDictionary<string, string>> node2AssigneeMap =
               new Dictionary<string, IDictionary<string, string>>();
          List < BpmAfExecution > executions = new List<BpmAfExecution>();
          for (var i = 0; i < firstAssigneeNodes.Count; i++)
          {
               BpmnConfCommonElementVo firstAssigneeNode = firstAssigneeNodes[i]; 
                 IDictionary<string, string> assigneeMap =firstAssigneeNode.AssigneeMap;
               string executionId = StrongUuidGenerator.GetNextId();
               DateTime nowTime = DateTime.Now;
               int signType = firstAssigneeNode.SignType;
               int taskCount =
                    signType == SignTypeEnum.SIGN_TYPE_OR_SIGN.GetCode() ||
                    signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()
                         ? 1
                         : assigneeMap.Count;
               BpmAfExecution execution = new BpmAfExecution
               {
                    Id = executionId,
                    ProcInstId = procInstId,
                    BusinessKey = bpmnConfCommonVo.FormCode,
                    ProcDefId = deploymentId,
                    ActId = firstAssigneeNode.ElementId,
                    Name = firstAssigneeNode.ElementName,
                    StartTime = nowTime,
                    StartUserId = SecurityUtils.GetLogInEmpId(),
                    TaskCount = taskCount,
                    SignType = signType
               };
               executions.Add(execution);
               _executionService.baseRepo.Insert(execution);
               List<BpmAfTask> tasks = new List<BpmAfTask>();
               List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
               if (i == 0)
               {
                    BpmAfTaskInst startTask = new BpmAfTaskInst
                    {
                         Id = StrongUuidGenerator.GetNextId(),
                         ProcInstId = procInstId,
                         ProcDefId = deploymentId,
                         ExecutionId = executionId,
                         Name = StringConstants.START_USER_NODE_NAME,
                         TaskDefKey = ProcessNodeEnum.START_TASK_KEY.Description,
                         Owner = bpmnStartConditions.StartUserId,
                         Assignee = bpmnStartConditions.StartUserId,
                         AssigneeName = bpmnStartConditions.StartUserName,
                         StartTime = nowTime,
                         EndTime = nowTime,
                         Priority = 0,
                         DeleteReason = "发起人节点自动完成",
                         FormKey = bpmnConfCommonVo.FormCode,
                    };
                    historyTaskInsts.Add(startTask);
               }
               foreach (var (key, value) in assigneeMap)
               {
                    BpmAfTask bpmAfTask = new BpmAfTask()
                    {
                         Id = StrongUuidGenerator.GetNextId(),
                         ProcInstId = procInstId,
                         ProcDefId = deploymentId,
                         ExecutionId = executionId,
                         Name = firstAssigneeNode.ElementName,
                         TaskDefKey = firstAssigneeNode.ElementId,
                         Owner = bpmnStartConditions.StartUserId,
                         Assignee = key,
                         AssigneeName = value,
                         CreateTime = nowTime.AddSeconds(1),
                         FormKey = bpmnConfCommonVo.FormCode,
                    };
                    tasks.Add(bpmAfTask);
                    if (signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
                    {
                         break;
                    }
               }
               
               _afTaskService.InsertTasks(tasks);
               foreach (BpmAfTask bpmAfTask in tasks)
               {
                    historyTaskInsts.Add(bpmAfTask.ToInst());
               }

               _taskInstService.baseRepo.Insert(historyTaskInsts);
          }

          ExecutionEntity executionEntity = new ExecutionEntity()
          {
               Name = executions[0].Name,
               ProcessInstanceId = procInstId,
          };
          return executionEntity;
     }
     
     public void InsertTasks(BpmBusinessProcess bpmBusinessProcess,string taskDefKey)
     {
          string procInstId = bpmBusinessProcess.ProcInstId;
          string businessKey = bpmBusinessProcess.ProcessinessKey;
          DateTime nowTime = DateTime.Now;

          List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo
               .Where(a=>a.ProcInstId==procInstId&&a.TaskDefKey==taskDefKey).ToList();

          if (bpmAfTaskInsts.IsEmpty())
          {
               throw new AFBizException(BusinessError.STATUS_ERROR, "未能找到流程信息");
          }

          string procDefId = bpmAfTaskInsts[0].ProcDefId;
          BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id==procDefId).First();
          if (bpmAfDeployment == null)
          {
               throw new ApplicationException($"deployment with id {procDefId} not found");
          }
          string content = bpmAfDeployment.Content;
          List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
          BpmnConfCommonElementVo? bpmnConfCommonElementVo = elements.FirstOrDefault(a => a.ElementId==taskDefKey);
          if (bpmnConfCommonElementVo == null)
          {
               throw new AFBizException(BusinessError.STATUS_ERROR,"未能找到流程定义信息!");
          }
          string newExecutionId=StrongUuidGenerator.GetNextId();
          string deploymentId = bpmAfDeployment.Id;
          int signType = bpmnConfCommonElementVo.SignType;
          BpmAfExecution execution = new BpmAfExecution
          {
               Id = newExecutionId,
               ProcInstId = procInstId,
               BusinessKey = businessKey,
               ProcDefId = procDefId,
               ActId = taskDefKey,
               Name = bpmnConfCommonElementVo.ElementName,
               StartTime = nowTime,
               StartUserId = SecurityUtils.GetLogInEmpId(),
               SignType =signType,
               TaskCount = bpmnConfCommonElementVo.AssigneeMap.Count,
               TenantId = MultiTenantUtil.GetCurrentTenantId(),
          };
          IDictionary<string,string> assigneeMap = bpmnConfCommonElementVo.AssigneeMap;
          
          List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
          List<BpmAfTask> tasks = new List<BpmAfTask>();

          foreach (var (key, value) in assigneeMap)
          {
               BpmAfTask bpmAfTask = new BpmAfTask()
               {
                    Id = StrongUuidGenerator.GetNextId(),
                    ProcInstId = procInstId,
                    ProcDefId = deploymentId,
                    ExecutionId = newExecutionId,
                    Name = bpmnConfCommonElementVo.ElementName,
                    TaskDefKey = bpmnConfCommonElementVo.ElementId,
                    Owner = bpmBusinessProcess.CreateUser,
                    Assignee = key,
                    AssigneeName = value,
                    CreateTime = nowTime.AddSeconds(1),
                    FormKey = bpmBusinessProcess.ProcessinessKey,
               };
               tasks.Add(bpmAfTask);
               if (signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
               {
                    break;
               }
          }
          _afTaskService.InsertTasks(tasks);
          foreach (BpmAfTask afTask in tasks)
          {
               BpmAfTaskInst bpmAfTaskInst = afTask.ToInst();
               historyTaskInsts.Add(bpmAfTaskInst);
          }
          
          _afTaskInstService.baseRepo.Insert(historyTaskInsts);
        
     }
}