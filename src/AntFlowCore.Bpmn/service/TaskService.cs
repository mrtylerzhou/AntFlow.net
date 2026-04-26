using System.Linq.Expressions;
using System.Text.Json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.listener;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.service;

public class TaskService : ITaskService
{
    private readonly IAFTaskService _afTaskService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IAFExecutionService _afExecutionService;
    private readonly IExecutionListener _executionListener;
    private readonly IBpmVariableSignUpPersonnelService _signUpPersonnelService;
    private readonly IAFDeploymentService _afDeploymentService;

    public TaskService(
        IAFTaskService afTaskService,
        IAfTaskInstService afTaskInstService,
        IAFExecutionService afExecutionService,
        IExecutionListener executionListener,
        IBpmVariableSignUpPersonnelService signUpPersonnelService,
        IAFDeploymentService afDeploymentService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _afExecutionService = afExecutionService;
        _executionListener = executionListener;
        _signUpPersonnelService = signUpPersonnelService;
        _afDeploymentService = afDeploymentService;
    }
    public List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask,bool>> filter)
    {
        List<BpmAfTask> bpmAfTasks = _afTaskService._repository.Find(filter);
        return bpmAfTasks;
    }

    
    public void Complete( BpmAfTask task)
    {
       
        DateTime nowTime = DateTime.Now;
        string deleteReason = StringConstants.DEFAULT_TASK_DELETE_REASON;
        string taskId = task.Id;
        string procDefId = task.ProcDefId;
        string taskDefKey = task.TaskDefKey;
        string procInstId = task.ProcInstId;
        List<BpmAfTask> afTasks = _afTaskService._repository.Find(a => a.ProcInstId == procInstId);
        BpmAfTask bpmAfTask = afTasks.First(a => a.Id == taskId);
      
        if (bpmAfTask == null)
        {
            throw new ApplicationException($"Task with id {taskId} not found");
        }
        BpmAfExecution currentExecution = _afExecutionService._repository.FirstOrDefault(a=>a.Id==bpmAfTask.ExecutionId&&a.ActId==task.TaskDefKey);
        if (currentExecution == null)
        {
            throw new AFBizException("未能找到当前流程执行实例!");
        }
        int? currentSignType = currentExecution.SignType??1;
        
       
        if (currentSignType == 2||task.IsNextNodeSignUp)
        {
            _afTaskService._repository.DeleteByExpression(a => a.TaskDefKey == taskDefKey&&a.ProcInstId==procInstId);
        }
        else
        {
            _afTaskService._repository.DeleteByExpression(a=>a.Id==taskId);
        }
        TimeSpan taskDuration = nowTime - bpmAfTask.CreateTime;
        int durationMinutes = taskDuration.Minutes;
        bool isCopyNode = bpmAfTask.NodeType==(int)NodeTypeEnum.NODE_TYPE_COPY;
        _afTaskInstService._repository.UpdateTaskDurationAndEndTime(
            taskId, durationMinutes, nowTime, deleteReason, isCopyNode,
            isCopyNode ? task.Assignee : null, isCopyNode ? task.AssigneeName : null);
        BpmAfDeployment bpmAfDeployment = _afDeploymentService._repository.FirstOrDefault(a=>a.Id==procDefId);
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procDefId} not found");
        }
        
        if (currentExecution.TaskCount!.Value >= 2)
        {
            int currentCount = currentExecution.TaskCount - 1 ?? 0;
            _afExecutionService._repository.UpdateTaskCount(currentExecution.Id, currentCount);
            return;
        }
        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        List<BpmnConfCommonElementVo> elementToDealList = new List<BpmnConfCommonElementVo>();
        BpmnConfCommonElementVo elementToDeal = null;
        List<string> verifyUserIds = new List<string>();
        if (currentSignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
        {
            BpmnConfCommonElementVo? currentElement = BpmnFlowUtil.GetCurrentTaskElement(elements,taskDefKey);
            if (currentElement == null)
            {
                throw new AFBizException($"can not get current element by element id: {currentElement}");
            }

            IBpmVerifyInfoService bpmVerifyInfoService = ServiceProviderUtils.GetService<IBpmVerifyInfoService>();
            List<BpmVerifyInfo> bpmVerifyInfos = bpmVerifyInfoService._repository
                .FindByRunInfoIdAndTaskDefKey(procInstId, taskDefKey);
            verifyUserIds= bpmVerifyInfos.Select(a=>a.VerifyUserId).ToList();
            int currentNodeAssigneesCount = currentElement.AssigneeMap.Count;
            if (verifyUserIds.Count != currentNodeAssigneesCount)
            {
                elementToDeal = currentElement;
            }
            else
            {
                var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements,taskDefKey);
                elementToDeal=nextUserElement;
            }
           
        }
        else
        {
            var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements,taskDefKey);
            if (nextUserElement.FlowTo == nextFlowElement.FlowTo)
            { 
                
                string flowTo = nextFlowElement.FlowTo;

                elementToDeal = elements.First(a => a.ElementId == flowTo);
            }
            else
            {
                elementToDeal=nextUserElement;
            }
           
        }

        if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
        {
            List<BpmAfTask> otherNodeTasks = afTasks.Where(a => a.Id != taskId).ToList();
            if(otherNodeTasks.Count>0)
            {
                return;
            }

            List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = BpmnFlowUtil.GetNodeFromCurrentNexts(elements,elementToDeal.ElementId);
            elementToDealList.AddRange(bpmnConfCommonElementVos);
        }

        if (elementToDealList.Count <= 0)
        {
            elementToDealList.Add(elementToDeal);
        }

        foreach (BpmnConfCommonElementVo bpmnConfCommonElementVo in elementToDealList)
        {
            elementToDeal = bpmnConfCommonElementVo;
            IDictionary<string, string> assigneeMap = elementToDeal.AssigneeMap;
            if (elementToDeal.IsSignUpSubElement == 1)
            {
                
                List<KeyValuePair<string, string>> signupNodeAssigneeMap = this._signUpPersonnelService._repository
                    .GetSignUpNodeAssigneeMap(procInstId, elementToDeal.ElementId);
                if (signupNodeAssigneeMap.Count <= 0&& elementToDeal.IsBackSignUp!=1)
                {
                    var (nextUserElement, nextFlowElement) = GetNextAssigneeNodeRecursively(elements, elementToDeal);
                    if (nextUserElement == null)
                    {
                        long count = _afTaskService._repository.Count(a=>a.ProcInstId==procInstId);
                        if (count > 0)
                        {
                            return;
                        }

                        elementToDeal = BpmnFlowUtil.GetAggNode(elements, elementToDeal);
                    }
                    else
                    {
                        BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementToDeal.ElementId);
                        elementToDeal = nextUserElement;
                    }
                    assigneeMap = elementToDeal.AssigneeMap;
                }
                else
                {
                    if (elementToDeal.IsBackSignUp == 1)
                    {
                        BpmnConfCommonElementVo? confCommonElementVo = elements
                            .FirstOrDefault(a => a.CollectionName==elementToDeal.CollectionName&&a.ElementId!=elementToDeal.ElementId);
                        if (confCommonElementVo == null)
                        {
                            throw new AFBizException(BusinessError.DATA_NOT_FOUND, "未能找到加批原节点,请联系管理员");
                        } 
                        assigneeMap=confCommonElementVo.AssigneeMap;
                    }
                    else
                    {
                        assigneeMap = signupNodeAssigneeMap.ToDictionary(k => k.Key, v => v.Value);
                    }
                }
            }

            int taskCount = elementToDeal.SignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()
                ? 1
                : assigneeMap?.Count ?? 0;
            string newExecutionId=StrongUuidGenerator.GetNextId();
            BpmAfExecution execution = new BpmAfExecution
            {
                Id = newExecutionId,
                ProcInstId = procInstId,
                BusinessKey = currentExecution.BusinessKey,
                ProcDefId = procDefId,
                ActId = elementToDeal.ElementId,
                Name = elementToDeal.ElementName,
                StartTime = nowTime,
                StartUserId = SecurityUtils.GetLogInEmpId(),
                SignType =elementToDeal.SignType,
                TaskCount = taskCount,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _afExecutionService._repository.Add(execution);
            if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
            {
                deleteReason = StringConstants.TASK_FINISH_REASON;
                _executionListener.Notify(execution, IExecutionListener.EVENTNAME_END);
            }
            
           
            if (deleteReason.Equals(StringConstants.TASK_FINISH_REASON))
            {
                return;
            }

            List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
            List<BpmAfTask> tasks = new List<BpmAfTask>();

            foreach (var (key, value) in assigneeMap)
            {
                if (verifyUserIds.Contains(key))
                {
                    continue;
                }

                BpmAfTask newTask = new BpmAfTask()
                {
                    Id = StrongUuidGenerator.GetNextId(),
                    ProcInstId = procInstId,
                    ProcDefId = procDefId,
                    ExecutionId = newExecutionId,
                    Name = elementToDeal.ElementName,
                    TaskDefKey = elementToDeal.ElementId,
                    NodeId = elementToDeal.NodeId,
                    NodeType = elementToDeal.NodeType,
                    Owner = bpmAfTask.Owner,
                    Assignee = key,
                    AssigneeName = value,
                    CreateTime = nowTime,
                    FormKey = bpmAfTask.FormKey,
                    TenantId = MultiTenantUtil.GetCurrentTenantId(),
                };
                tasks.Add(newTask);
                if (elementToDeal.SignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
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

            _afExecutionService._repository.DeleteByExpression(a => a.Id == currentExecution.Id);
            _afTaskInstService._repository.AddRange(historyTaskInsts);
        }
       
    }

    private (BpmnConfCommonElementVo assigneeNode,BpmnConfCommonElementVo flowNode) GetNextAssigneeNodeRecursively(List<BpmnConfCommonElementVo> elements,BpmnConfCommonElementVo elementToDeal)
    {
        var (nextUserElement, nextFlowElement) =
            BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementToDeal.ElementId);
        if (nextUserElement!=null&&nextUserElement.AssigneeMap.IsEmpty() && nextFlowElement.FlowTo.IndexOf("EndEvent") == -1)
        {
          return  GetNextAssigneeNodeRecursively(elements,nextUserElement);
        }
        else
        {
            return (nextUserElement, nextFlowElement);
        }
    }
}