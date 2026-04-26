using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class ProcessNodeJumpService : IProcessNodeJumpService
{
    private readonly IAFTaskService _afTaskService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IAFExecutionService _executionService;
    private readonly IAFDeploymentService _afDeploymentService;
    private readonly ITaskService _taskService;

    public ProcessNodeJumpService(
        IAFTaskService afTaskService,
        IAfTaskInstService afTaskInstService,
        IAFExecutionService executionService,
        IAFDeploymentService afDeploymentService,
        ITaskService taskService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _executionService = executionService;
        _afDeploymentService = afDeploymentService;
        _taskService = taskService;
    }
    public void CommitProcess(BpmAfTask task, Dictionary<String, Object> variables,
        String backNodeKey)
    {
        if (variables == null)
        {
            variables = new Dictionary<string, object>();
        }

        if (string.IsNullOrEmpty(backNodeKey))
        {
            _taskService.Complete(task);
        }
        else
        {
            TurnTransition(task.Id, backNodeKey, variables);
        }
    }
    public void TurnTransition(String taskId, String taskToTurnToNodeKey, Dictionary<String, Object> variables)
    {
        BpmAfTask bpmAfTask = _afTaskService._repository.FirstOrDefault(a => a.Id == taskId);
        if (bpmAfTask == null)
        {
            throw new AFBizException($"can not find task by id: {taskId}");
        }
        TurnTransition(bpmAfTask, taskToTurnToNodeKey, null, variables);
    }
    public void TurnTransition(BpmAfTask bpmAfTask, String taskToTurnToNodeKey, BpmAfDeployment bpmAfDeployment, Dictionary<String, Object> variables)
    {
        string verifyComment = variables.ContainsKey(StringConstants.VERIFY_COMMENT) ? variables[StringConstants.VERIFY_COMMENT]?.ToString() : "";
        DateTime nowTime = DateTime.Now;
        _afTaskInstService._repository
            .UpdateTaskInstByTaskId(bpmAfTask.Id, StringConstants.DEFAULT_TASK_DELETE_REASON, (int)ProcessSubmitStateEnum.PROCESS_UPDATE_TYPE, verifyComment, nowTime, (nowTime - bpmAfTask.CreateTime).Seconds);

        string procInstId = bpmAfTask.ProcInstId;
        string procDefId = bpmAfTask.ProcDefId;


        if (bpmAfDeployment == null)
        {
            bpmAfDeployment = _afDeploymentService._repository.FirstOrDefault(a => a.Id == procDefId);
        }

        if (bpmAfDeployment == null)
        {
            throw new AFBizException($"can not find deployment by id: {procDefId}");
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        BpmnConfCommonElementVo turnToElement = elements.First(a => a.ElementId == taskToTurnToNodeKey);
        int signType = turnToElement.SignType;
        IDictionary<string, string> assigneeMap = turnToElement.AssigneeMap;
        string executionId = StrongUuidGenerator.GetNextId();

        _executionService._repository.DeleteByExpression(a => a.Id == bpmAfTask.ExecutionId);

        List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
        List<BpmAfTask> tasks = new List<BpmAfTask>();
        int index = 0;
        foreach (var (key, value) in assigneeMap)
        {
            if (index > 0 && signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
            {
                break;
            }
            BpmAfTask newTask = new BpmAfTask()
            {
                Id = StrongUuidGenerator.GetNextId(),
                ProcInstId = procInstId,
                ProcDefId = procDefId,
                ExecutionId = executionId,
                Name = turnToElement.ElementName,
                TaskDefKey = turnToElement.ElementId,
                Owner = bpmAfTask.Owner,
                Assignee = key,
                AssigneeName = value,
                CreateTime = nowTime,
                FormKey = bpmAfTask.FormKey,
            };
            tasks.Add(newTask);
            BpmAfTaskInst bpmAfTaskInst = newTask.ToInst();
            TimeSpan taskDuration = nowTime - bpmAfTask.CreateTime;
            bpmAfTaskInst.Duration = taskDuration.Minutes;
            bpmAfTaskInst.EndTime = nowTime;
            bpmAfTaskInst.Description = StringConstants.BACK_TO_MODIFY_DESC;
            historyTaskInsts.Add(bpmAfTaskInst);
            index++;
        }
        BpmAfExecution execution = new BpmAfExecution
        {
            Id = executionId,
            ProcInstId = procInstId,
            //BusinessKey = bpmnStartConditions.BusinessId, //todo注意观察此字段更新时是否会丢失
            ProcDefId = procDefId,
            ActId = turnToElement.ElementId,
            Name = turnToElement.ElementName,
            StartTime = nowTime,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount = signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode() ? 1 : assigneeMap.Count,
        };
        _executionService._repository.Add(execution);
        _afTaskService._repository.DeleteByExpression(a => a.Id == bpmAfTask.Id);
        _afTaskService.InsertTasks(tasks);
        //_afTaskInstService.baseRepo.Insert(historyTaskInsts);

    }

}