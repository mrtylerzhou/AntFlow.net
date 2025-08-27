using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using System.Collections.Concurrent;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessForwardService : AFBaseCurdRepositoryService<BpmProcessForward>, IBpmProcessForwardService
{
    private static readonly IDictionary<string, BpmProcessForward> processForwardMap =
        new ConcurrentDictionary<string, BpmProcessForward>();

    private static readonly IDictionary<string, BpmAfTask> taskMap = new ConcurrentDictionary<string, BpmAfTask>();
    private readonly AFTaskService _taskService;

    public BpmProcessForwardService(
        AFTaskService taskService,
        IFreeSql freeSql) : base(freeSql)
    {
        _taskService = taskService;
    }

    public void AddProcessForward(BpmProcessForward bpmProcessForward)
    {
        baseRepo.Insert(bpmProcessForward);
    }

    public void UpdateProcessForward(BpmProcessForward bpmProcessForward)
    {
        List<BpmProcessForward> bpmProcessForwards = baseRepo.Where(a =>
                a.ProcessInstanceId == bpmProcessForward.ProcessInstanceId &&
                a.ForwardUserId == bpmProcessForward.ForwardUserId)
            .ToList();
        foreach (BpmProcessForward processForward in bpmProcessForwards)
        {
            baseRepo.Update(processForward);
        }
    }

    public void LoadProcessForward(string userId)
    {
        List<BpmProcessForward> list = AllBpmProcessForward(userId);
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
        List<BpmProcessForward> bpmProcessForwards = baseRepo
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

    public BpmAfTask GetTask(string processInstanceId)
    {
        if (string.IsNullOrEmpty(processInstanceId))
        {
            return null;
        }

        taskMap.TryGetValue(processInstanceId, out BpmAfTask? task);
        return task;
    }

    public BpmProcessForward GetProcessForward(string processInstanceId)
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