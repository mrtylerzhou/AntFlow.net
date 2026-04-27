using System.Collections.Concurrent;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmProcessForwardService : IBpmProcessForwardService
{
    private readonly AFTaskService _taskService;

    public BpmProcessForwardService(
        AFTaskService taskService,
        IBpmProcessForwardRepository repository)
    {
        _taskService = taskService;
        _repository = repository;
    }

    public IBpmProcessForwardRepository _repository { get; }

    private static IDictionary<String, BpmProcessForward> processForwardMap = new ConcurrentDictionary<String, BpmProcessForward>();
    private static IDictionary<String, BpmAfTask> taskMap = new ConcurrentDictionary<String, BpmAfTask>();

    public void AddProcessForward(BpmProcessForward bpmProcessForward)
    {
        _repository.Add(bpmProcessForward);
    }

    public void UpdateProcessForward(BpmProcessForward bpmProcessForward)
    {
        List<BpmProcessForward> bpmProcessForwards = _repository
            .Find(a => a.ProcessInstanceId == bpmProcessForward.ProcessInstanceId && a.ForwardUserId == bpmProcessForward.ForwardUserId);
        foreach (BpmProcessForward processForward in bpmProcessForwards)
        {
            _repository.Update(processForward);
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
        List<BpmProcessForward> bpmProcessForwards = _repository
            .Find(a => a.IsDel == 0 && a.ForwardUserId == userId);
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
