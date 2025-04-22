using antflowcore.entity;
using AntFlowCore.Entity;
using System.Collections.Concurrent;

namespace antflowcore.service.repository;

public class BpmProcessForwardService : AFBaseCurdRepositoryService<BpmProcessForward>
{
    private readonly AFTaskService _taskService;

    public BpmProcessForwardService(
        AFTaskService taskService,
        IFreeSql freeSql) : base(freeSql)
    {
        _taskService = taskService;
    }

    private static IDictionary<String, BpmProcessForward> processForwardMap = new ConcurrentDictionary<String, BpmProcessForward>();
    private static IDictionary<String, BpmAfTask> taskMap = new ConcurrentDictionary<String, BpmAfTask>();

    public void AddProcessForward(BpmProcessForward bpmProcessForward)
    {
        this.baseRepo.Insert(bpmProcessForward);
    }

    public void UpdateProcessForward(BpmProcessForward bpmProcessForward)
    {
        List<BpmProcessForward> bpmProcessForwards = this
            .baseRepo.
            Where(a => a.ProcessInstanceId == bpmProcessForward.ProcessInstanceId && a.ForwardUserId == bpmProcessForward.ForwardUserId)
            .ToList();
        foreach (BpmProcessForward processForward in bpmProcessForwards)
        {
            this.baseRepo.Update(processForward);
        }
    }

    public void LoadProcessForward(string userId)
    {
        List<BpmProcessForward> list = this.AllBpmProcessForward(userId);
        if (list == null || !list.Any())
        {
            return;
        }
        foreach (BpmProcessForward next in list)
        {
            if (!processForwardMap.ContainsKey(next.ProcessInstanceId))
            {
                processForwardMap.Add(next.ProcessInstanceId, next);
            }
        }
    }

    private List<BpmProcessForward> AllBpmProcessForward(string userId)
    {
        List<BpmProcessForward> bpmProcessForwards = this.baseRepo
            .Where(a => a.IsDel == 0 && a.ForwardUserId == userId)
            .ToList();
        return bpmProcessForwards;
    }

    public void LoadTask(string userId)
    {
        List<BpmAfTask> list = _taskService.FindTaskByEmpId(userId);
        if (list == null || !list.Any())
        {
            return;
        }
        foreach (BpmAfTask next in list)
        {
            if (!taskMap.ContainsKey(next.ProcInstId))
            {
                taskMap.Add(next.ProcInstId, next);
            }
        }
    }

    public BpmAfTask GetTask(String processInstanceId)
    {
        if (string.IsNullOrEmpty(processInstanceId))
        {
            return null;
        }

        taskMap.TryGetValue(processInstanceId, out BpmAfTask? task);
        return task;
    }

    public BpmProcessForward GetProcessForward(String processInstanceId)
    {
        if (string.IsNullOrEmpty(processInstanceId))
        {
            return null;
        }
        processForwardMap.TryGetValue(processInstanceId, out BpmProcessForward? forward);
        return forward;
    }

    public bool IsForward(string recordProcessInstanceId)
    {
        BpmAfTask bpmAfTask = GetTask(recordProcessInstanceId);
        if (bpmAfTask == null)
        {
            return false;
        }

        BpmProcessForward bpmProcessForward = GetProcessForward(recordProcessInstanceId);
        if (bpmProcessForward == null)
        {
            return false;
        }

        return true;
    }
}