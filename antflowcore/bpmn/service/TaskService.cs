using antflowcore.bpmn.listener;
using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using System.Linq.Expressions;
using System.Text.Json;

namespace antflowcore.bpmn.service;

public class TaskService
{
    private readonly AFTaskService _afTaskService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFExecutionService _afExecutionService;
    private readonly IExecutionListener _executionListener;
    private readonly AFDeploymentService _afDeploymentService;

    public TaskService(
        AFTaskService afTaskService,
        AfTaskInstService afTaskInstService,
        AFExecutionService afExecutionService,
        IExecutionListener executionListener,
        AFDeploymentService afDeploymentService)
    {
        _afTaskService = afTaskService;
        _afTaskInstService = afTaskInstService;
        _afExecutionService = afExecutionService;
        _executionListener = executionListener;
        _afDeploymentService = afDeploymentService;
    }

    public List<BpmAfTask> CreateTaskQuery(Expression<Func<BpmAfTask, bool>> filter)
    {
        List<BpmAfTask> bpmAfTasks = _afTaskService.baseRepo.Where(filter).ToList();
        return bpmAfTasks;
    }

    public void Complete(string taskId)
    {
        DateTime nowTime = DateTime.Now;
        string deleteReason = StringConstants.DEFAULT_TASK_DELETE_REASON;
        BpmAfTask bpmAfTask = _afTaskService.baseRepo.Where(a => a.Id == taskId).First();
        if (bpmAfTask == null)
        {
            throw new ApplicationException($"Task with id {taskId} not found");
        }
        string procDefId = bpmAfTask.ProcDefId;
        string taskDefKey = bpmAfTask.TaskDefKey;
        string procInstId = bpmAfTask.ProcInstId;
        _afTaskService.baseRepo.Delete(bpmAfTask);
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a => a.Id == procDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procDefId} not found");
        }

        BpmAfExecution currentExecution = _afExecutionService.baseRepo.Where(a => a.Id == bpmAfTask.ExecutionId).First();
        if (currentExecution == null)
        {
            throw new AFBizException("未能找到当前流程执行实例!");
        }

        if (currentExecution.TaskCount!.Value >= 2)
        {
            BpmAfExecution afExecution = new BpmAfExecution
            {
                Id = currentExecution.Id,
                TaskCount = currentExecution.TaskCount - 1
            };
            _afExecutionService.baseRepo.Update(afExecution);
            return;
        }
        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        var (nextUserElement, nextFlowElement) = BpmnFlowUtil.GetNextAssigneeAndFlowNode(elements, taskDefKey);

        Dictionary<string, string> assigneeMap = nextUserElement.AssigneeMap;
        BpmAfExecution execution = new BpmAfExecution
        {
            Id = bpmAfTask.ExecutionId,
            ProcInstId = procInstId,
            //BusinessKey = bpmnStartConditions.BusinessId, //todo注意观察此字段更新时是否会丢失
            ProcDefId = procDefId,
            ActId = nextUserElement.ElementId,
            Name = nextUserElement.ElementName,
            StartTime = nowTime,
            StartUserId = SecurityUtils.GetLogInEmpId(),
            TaskCount = assigneeMap?.Count,
        };

        _afExecutionService.baseRepo.Update(execution);
        if (nextUserElement.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
        {
            deleteReason = StringConstants.TASK_FINISH_REASON;
            _executionListener.Notify(execution, IExecutionListener.EVENTNAME_END);
        }
        _afTaskInstService.Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.EndTime, nowTime)
            .Set(a => a.DeleteReason, deleteReason)
            .Where(a => a.Id == taskId)
            .ExecuteAffrows();
        if (deleteReason.Equals(StringConstants.TASK_FINISH_REASON))
        {
            return;
        }
        List<BpmAfTaskInst> historyTaskInsts = new List<BpmAfTaskInst>();
        List<BpmAfTask> tasks = new List<BpmAfTask>();
        foreach (var (key, value) in assigneeMap)
        {
            BpmAfTask newTask = new BpmAfTask()
            {
                Id = StrongUuidGenerator.GetNextId(),
                ProcInstId = procInstId,
                ProcDefId = procDefId,
                ExecutionId = bpmAfTask.ExecutionId,
                Name = nextUserElement.ElementName,
                TaskDefKey = nextUserElement.ElementId,
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
            historyTaskInsts.Add(bpmAfTaskInst);
        }
        _afTaskService.baseRepo.Insert(tasks);
        _afTaskInstService.baseRepo.Insert(historyTaskInsts);
    }
}