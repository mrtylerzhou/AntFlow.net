using AntFlow.Core.Bpmn.Listener;
using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql;
using System.Linq.Expressions;
using System.Text.Json;

namespace AntFlow.Core.Bpmn.Service;

public class TaskService
{
    private readonly AFDeploymentService _afDeploymentService;
    private readonly AFExecutionService _afExecutionService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFTaskService _afTaskService;
    private readonly IExecutionListener _executionListener;
    private readonly BpmVariableSignUpPersonnelService _signUpPersonnelService;

    public TaskService(
        AFTaskService afTaskService,
        AfTaskInstService afTaskInstService,
        AFExecutionService afExecutionService,
        IExecutionListener executionListener,
        BpmVariableSignUpPersonnelService signUpPersonnelService,
        AFDeploymentService afDeploymentService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _afExecutionService = afExecutionService;
        _executionListener = executionListener;
        _signUpPersonnelService = signUpPersonnelService;
        _afDeploymentService = afDeploymentService;
    }

    public List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask, bool>> filter)
    {
        List<BpmAfTask> bpmAfTasks = _afTaskService.baseRepo.Where(filter).ToList();
        return bpmAfTasks;
    }


    public void Complete(BpmAfTask task)
    {
        DateTime nowTime = DateTime.Now;
        string deleteReason = StringConstants.DEFAULT_TASK_DELETE_REASON;
        string taskId = task.Id;
        string procDefId = task.ProcDefId;
        string taskDefKey = task.TaskDefKey;
        string procInstId = task.ProcInstId;
        List<BpmAfTask> afTasks = _afTaskService.baseRepo.Where(a => a.ProcInstId == procInstId).ToList();
        BpmAfTask bpmAfTask = afTasks.First(a => a.Id == taskId);

        if (bpmAfTask == null)
        {
            throw new ApplicationException($"Task with id {taskId} not found");
        }

        BpmAfExecution currentExecution = _afExecutionService.baseRepo
            .Where(a => a.Id == bpmAfTask.ExecutionId && a.ActId == task.TaskDefKey).First();
        if (currentExecution == null)
        {
            throw new AFBizException("¦Ä??????????????????!");
        }

        int? currentSignType = currentExecution.SignType ?? 1;


        if (currentSignType == 2 || task.IsNextNodeSignUp)
        {
            _afTaskService.Frsql.Delete<BpmAfTask>()
                .Where(a => a.TaskDefKey == taskDefKey && a.ProcInstId == procInstId)
                .ExecuteAffrows();
        }
        else
        {
            _afTaskService.Frsql.Delete<BpmAfTask>().Where(a => a.Id == taskId).ExecuteAffrows();
        }

        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a => a.Id == procDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procDefId} not found");
        }

        if (currentExecution.TaskCount!.Value >= 2)
        {
            int currentCount = currentExecution.TaskCount - 1 ?? 0;
            _afExecutionService.Frsql.Update<BpmAfExecution>()
                .Set(a => a.TaskCount, currentCount)
                .Where(a => a.Id == currentExecution.Id)
                .ExecuteAffrows();
            return;
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        List<BpmnConfCommonElementVo> elementToDealList = new();
        BpmnConfCommonElementVo elementToDeal = null;
        List<string> verifyUserIds = new();
        if (currentSignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
        {
            BpmnConfCommonElementVo currentElement = BpmnFlowUtil.GetCurrentTaskElement(elements, taskDefKey);
            if (currentElement == null)
            {
                throw new AFBizException($"can not get current element by element id: {currentElement}");
            }

            BpmVerifyInfoService bpmVerifyInfoService = ServiceProviderUtils.GetService<BpmVerifyInfoService>();
            List<BpmVerifyInfo> bpmVerifyInfos = bpmVerifyInfoService.baseRepo
                .Where(a => a.RunInfoId == procInstId && a.TaskDefKey == taskDefKey)
                .ToList();
            verifyUserIds = bpmVerifyInfos.Select(a => a.VerifyUserId).ToList();
            int currentNodeAssigneesCount = currentElement.AssigneeMap.Count;
            if (verifyUserIds.Count != currentNodeAssigneesCount)
            {
                elementToDeal = currentElement;
            }
            else
            {
                (BpmnConfCommonElementVo? nextUserElement, BpmnConfCommonElementVo? nextFlowElement) =
                    BpmnFlowUtil.GetNextNodeAndFlowNode(elements, taskDefKey);
                elementToDeal = nextUserElement;
            }
        }
        else
        {
            (BpmnConfCommonElementVo? nextUserElement, BpmnConfCommonElementVo? nextFlowElement) =
                BpmnFlowUtil.GetNextNodeAndFlowNode(elements, taskDefKey);
            if (nextUserElement.FlowTo == nextFlowElement.FlowTo)
            {
                string flowTo = nextFlowElement.FlowTo;

                elementToDeal = elements.First(a => a.ElementId == flowTo);
            }
            else
            {
                elementToDeal = nextUserElement;
            }
        }

        if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
        {
            List<BpmAfTask> otherNodeTasks = afTasks.Where(a => a.Id != taskId).ToList();
            if (otherNodeTasks.Count > 0)
            {
                return;
            }

            List<BpmnConfCommonElementVo> bpmnConfCommonElementVos =
                BpmnFlowUtil.GetNodeFromCurrentNexts(elements, elementToDeal.ElementId);
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
                List<KeyValuePair<string, string>> signupNodeAssigneeMap = _signUpPersonnelService.Frsql
                    .Select<BpmBusinessProcess, BpmVariable, BpmVariableSignUpPersonnel>()
                    .InnerJoin((a, b, c) => a.BusinessNumber == b.ProcessNum)
                    .InnerJoin((a, b, c) => b.Id == c.VariableId)
                    .Where((a, b, c) => a.ProcInstId == procInstId)
                    .ToList<KeyValuePair<string, string>>((a, b, c) =>
                        new KeyValuePair<string, string>(c.Assignee, c.AssigneeName));
                if (signupNodeAssigneeMap.Count <= 0)
                {
                    (BpmnConfCommonElementVo? nextUserElement, BpmnConfCommonElementVo? nextFlowElement) =
                        BpmnFlowUtil.GetNextNodeAndFlowNode(elements, elementToDeal.ElementId);
                    elementToDeal = nextUserElement;
                    assigneeMap = elementToDeal.AssigneeMap;
                }
                else
                {
                    assigneeMap = signupNodeAssigneeMap.ToDictionary(k => k.Key, v => v.Value);
                }
            }

            int taskCount = elementToDeal.SignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode()
                ? 1
                : assigneeMap?.Count ?? 0;
            string newExecutionId = StrongUuidGenerator.GetNextId();
            BpmAfExecution execution = new()
            {
                Id = newExecutionId,
                ProcInstId = procInstId,
                BusinessKey = currentExecution.BusinessKey,
                ProcDefId = procDefId,
                ActId = elementToDeal.ElementId,
                Name = elementToDeal.ElementName,
                StartTime = nowTime,
                StartUserId = SecurityUtils.GetLogInEmpId(),
                SignType = elementToDeal.SignType,
                TaskCount = taskCount,
                TenantId = MultiTenantUtil.GetCurrentTenantId()
            };

            _afExecutionService.baseRepo.Insert(execution);
            if (elementToDeal.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
            {
                deleteReason = StringConstants.TASK_FINISH_REASON;
                _executionListener.Notify(execution, IExecutionListener.EVENTNAME_END);
            }

            TimeSpan taskDuration = nowTime - bpmAfTask.CreateTime;
            int durationMinutes = taskDuration.Minutes;
            IUpdate<BpmAfTaskInst> update = _afTaskInstService.Frsql
                .Update<BpmAfTaskInst>()
                .Set(a => a.Duration, durationMinutes)
                .Set(a => a.EndTime, nowTime)
                .Set(a => a.DeleteReason, deleteReason);
            if (bpmAfTask.NodeType == (int)NodeTypeEnum.NODE_TYPE_COPY)
            {
                update.Set(a => a.Assignee, task.Assignee);
                update.Set(a => a.AssigneeName, task.AssigneeName);
            }

            update
                .Where(a => a.Id == taskId)
                .ExecuteAffrows();
            if (deleteReason.Equals(StringConstants.TASK_FINISH_REASON))
            {
                return;
            }

            List<BpmAfTaskInst> historyTaskInsts = new();
            List<BpmAfTask> tasks = new();

            foreach ((string? key, string? value) in assigneeMap)
            {
                if (verifyUserIds.Contains(key))
                {
                    continue;
                }

                BpmAfTask newTask = new()
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
                    TenantId = MultiTenantUtil.GetCurrentTenantId()
                };
                tasks.Add(newTask);
                if (currentSignType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
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

            _afTaskService.Frsql.Delete<BpmAfExecution>()
                .Where(a => a.Id == currentExecution.Id)
                .ExecuteAffrows();
            _afTaskInstService.baseRepo.Insert(historyTaskInsts);
        }
    }
}