using AntFlow.Core.Bpmn.Service;
using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class ProcessNodeJumpService
{
    private readonly AFDeploymentService _afDeploymentService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFTaskService _afTaskService;
    private readonly AFExecutionService _executionService;
    private readonly TaskService _taskService;

    public ProcessNodeJumpService(
        AFTaskService afTaskService,
        AfTaskInstService afTaskInstService,
        AFExecutionService executionService,
        AFDeploymentService afDeploymentService,
        TaskService taskService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _executionService = executionService;
        _afDeploymentService = afDeploymentService;
        _taskService = taskService;
    }

    public void CommitProcess(BpmAfTask task, Dictionary<string, object> variables,
        string backNodeKey)
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

    public void TurnTransition(string taskId, string taskToTurnToNodeKey, Dictionary<string, object> variables)
    {
        BpmAfTask bpmAfTask = _afTaskService.baseRepo.Where(a => a.Id == taskId).First();
        if (bpmAfTask == null)
        {
            throw new AFBizException($"can not find task by id: {taskId}");
        }

        TurnTransition(bpmAfTask, taskToTurnToNodeKey, null, variables);
    }

    public void TurnTransition(BpmAfTask bpmAfTask, string taskToTurnToNodeKey, BpmAfDeployment bpmAfDeployment,
        Dictionary<string, object> variables)
    {
        string verifyComment = variables.ContainsKey(StringConstants.VERIFY_COMMENT)
            ? variables[StringConstants.VERIFY_COMMENT].ToString()
            : "";
        DateTime nowTime = DateTime.Now;
        _afTaskInstService.Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.DeleteReason, StringConstants.DEFAULT_TASK_DELETE_REASON)
            .Set(a => a.VerifyStatus, (int)ProcessSubmitStateEnum.PROCESS_UPDATE_TYPE)
            .Set(a => a.VerifyDesc, verifyComment)
            .Set(a => a.EndTime, nowTime)
            .Set(a => a.Duration, (nowTime - bpmAfTask.CreateTime).Seconds)
            .Where(a => a.Id == taskToTurnToNodeKey)
            .ExecuteAffrows();

        string procInstId = bpmAfTask.ProcInstId;
        string procDefId = bpmAfTask.ProcDefId;


        if (bpmAfDeployment == null)
        {
            bpmAfDeployment = _afDeploymentService.baseRepo.Where(a => a.Id == procDefId).First();
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

        _executionService.Frsql
            .Delete<BpmAfExecution>()
            .Where(a => a.Id == bpmAfTask.ExecutionId)
            .ExecuteAffrows();

        List<BpmAfTaskInst> historyTaskInsts = new();
        List<BpmAfTask> tasks = new();
        int index = 0;
        foreach ((string? key, string? value) in assigneeMap)
        {
            if (index > 0 && signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode())
            {
                break;
            }

            BpmAfTask newTask = new()
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
                FormKey = bpmAfTask.FormKey
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

        BpmAfExecution execution = new()
        {
            Id = executionId,
            ProcInstId = procInstId,
            //BusinessKey = bpmnStartConditions.BusinessId, //todo????????¦È?????????
            ProcDefId = procDefId,
            ActId = turnToElement.ElementId,
            Name = turnToElement.ElementName,
            StartTime = nowTime,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount = signType == SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER.GetCode() ? 1 : assigneeMap.Count
        };
        _executionService.baseRepo.Insert(execution);
        _afTaskService.baseRepo.Delete(bpmAfTask);
        _afTaskService.baseRepo.Insert(tasks);
        //_afTaskInstService.baseRepo.Insert(historyTaskInsts);
    }
}