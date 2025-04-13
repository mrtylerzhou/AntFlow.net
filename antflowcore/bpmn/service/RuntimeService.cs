using antflowcore.bpmn.listener;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.bpmn.service;

public class RuntimeService
{
     private readonly AfTaskInstService _taskInstService;
     private readonly AFTaskService _taskService;
     private readonly ITaskListener _taskListener;
     private readonly AFExecutionService _executionService;

     public RuntimeService(
          AfTaskInstService taskInstService,
          AFTaskService taskService,
          ITaskListener taskListener,
          AFExecutionService executionService)
     {
          _taskInstService = taskInstService;
          _taskService = taskService;
          _taskListener = taskListener;
          _executionService = executionService;
     }
     
     public ExecutionEntity StartProcessInstance(BpmnConfCommonVo bpmnConfCommonVo,
          BpmnStartConditionsVo bpmnStartConditions,string deploymentId)
     {
          List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = bpmnConfCommonVo.ElementList;
          string procInstId = StrongUuidGenerator.GetNextId();
          string executionId= StrongUuidGenerator.GetNextId();
          BpmnConfCommonElementVo firtFirstAssigneeNode = BpmnFlowUtil.GetFirstAssigneeNode(bpmnConfCommonElementVos);
          Dictionary<string,string> assigneeMap = firtFirstAssigneeNode.AssigneeMap;
          DateTime nowTime = DateTime.Now;
          BpmAfExecution execution = new BpmAfExecution
          {
               Id = executionId,
               ProcInstId = procInstId,
               BusinessKey = bpmnConfCommonVo.FormCode,
               ProcDefId = deploymentId,
               ActId = firtFirstAssigneeNode.ElementId,
               Name = firtFirstAssigneeNode.ElementName,
               StartTime = nowTime,
               StartUserId = SecurityUtils.GetLogInEmpId(),
               TaskCount = assigneeMap.Count,
               
          };
          _executionService.baseRepo.Insert(execution);
          List<BpmAfTask> tasks = new List<BpmAfTask>();
          List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
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
               FormKey = bpmnConfCommonVo.FormCode,
          };
          historyTaskInsts.Add(startTask);
          foreach (var (key, value) in assigneeMap)
          {
               BpmAfTask bpmAfTask = new BpmAfTask()
               {
                    Id = StrongUuidGenerator.GetNextId(),
                    ProcInstId = procInstId,
                    ProcDefId = deploymentId,
                    ExecutionId = executionId,
                    Name = firtFirstAssigneeNode.ElementName,
                    TaskDefKey = firtFirstAssigneeNode.ElementId,
                    Owner = bpmnStartConditions.StartUserId,
                    Assignee = key,
                    AssigneeName = value,
                    CreateTime = nowTime,
                    FormKey = bpmnConfCommonVo.FormCode,
               };
               tasks.Add(bpmAfTask);
               historyTaskInsts.Add(bpmAfTask.ToInst());
          }
          _taskService.InsertTasks(tasks);
          _taskInstService.baseRepo.Insert(historyTaskInsts);
          ExecutionEntity executionEntity = new ExecutionEntity()
          {
               Name = execution.Name,
               ProcessInstanceId = procInstId,
          };
          return executionEntity;
     }
     
}