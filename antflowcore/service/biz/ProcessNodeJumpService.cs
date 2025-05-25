using System.Text.Json;
using antflowcore.bpmn.service;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class ProcessNodeJumpService
{
    private readonly AFTaskService _afTaskService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFExecutionService _executionService;
    private readonly AFDeploymentService _afDeploymentService;
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
    public void CommitProcess(BpmAfTask task, Dictionary<String, Object> variables,
        String backNodeKey)
    {
        if (variables == null) {
            variables = new Dictionary<string, object>();
        }

        if (string.IsNullOrEmpty(backNodeKey))
        {
            _taskService.Complete(task);
        }
        else
        {
            TurnTransition(task.Id, backNodeKey,variables);
        }
    }

    private void TurnTransition(String taskId, String taskToTurnToNodeKey, Dictionary<String, Object> variables)
    {
        BpmAfTask bpmAfTask = _afTaskService.baseRepo.Where(a => a.Id == taskId).First();
        if (bpmAfTask == null)
        {
            throw new AFBizException($"can not find task by id: {taskId}");
        }
        string verifyComment=variables.ContainsKey(StringConstants.VERIFY_COMMENT)?variables[StringConstants.VERIFY_COMMENT].ToString():"";
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

        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id == procDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new AFBizException($"can not find deployment by id: {procDefId}");
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        BpmnConfCommonElementVo turnToElement = elements.First(a => a.ElementId==taskToTurnToNodeKey);
        IDictionary<string,string> assigneeMap = turnToElement.AssigneeMap;
        BpmAfExecution execution = new BpmAfExecution
        {
            Id = bpmAfTask.ExecutionId,
            ProcInstId = procInstId,
            //BusinessKey = bpmnStartConditions.BusinessId, //todo注意观察此字段更新时是否会丢失
            ProcDefId = procDefId,
            ActId = turnToElement.ElementId,
            Name = turnToElement.ElementName,
            StartTime = nowTime,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount = assigneeMap?.Count,
        };
        _executionService.baseRepo.Update(execution);
        
        List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
        List<BpmAfTask> tasks=new List<BpmAfTask>();
        foreach (var (key, value) in assigneeMap)
        {
            BpmAfTask newTask = new BpmAfTask()
            {
                Id = StrongUuidGenerator.GetNextId(),
                ProcInstId = procInstId,
                ProcDefId = procDefId,
                ExecutionId = bpmAfTask.ExecutionId,
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
            TimeSpan taskDuration = nowTime-bpmAfTask.CreateTime;
            bpmAfTaskInst.Duration=taskDuration.Minutes;
            bpmAfTaskInst.EndTime=nowTime;
            bpmAfTaskInst.Description = StringConstants.BACK_TO_MODIFY_DESC;
            historyTaskInsts.Add(bpmAfTaskInst);
        }
        _afTaskService.baseRepo.Delete(bpmAfTask);
        _afTaskService.baseRepo.Insert(tasks);
        //_afTaskInstService.baseRepo.Insert(historyTaskInsts);
    }
}