using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmFlowrunEntrustService : IBpmFlowrunEntrustService
{
    public BpmFlowrunEntrustService(IBpmFlowrunEntrustRepository repository)
    {
        _repository = repository;
    }

    public IBpmFlowrunEntrustRepository _repository { get; }

    public BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId)
    {
        BpmFlowrunEntrust bpmFlowrunEntrust = _repository.FirstOrDefault(a =>
            a.Actual.Equals(actual) && a.RunInfoId.Equals(procDefId) && a.RunTaskId.Equals(taskId));
        return bpmFlowrunEntrust;
    }

    public void AddFlowrunEntrust(String actual, String actualName, String original, String originalName,
        String runtaskid, int type, String processInstanceId, String processKey, string nodeId, int actionType)
    {
        var entrust = new BpmFlowrunEntrust
        {
            Type = type,
            RunTaskId = runtaskid,
            Actual = actual,
            ActualName = actualName,
            Original = original,
            OriginalName = originalName,
            IsRead = 2,
            ProcDefId = processKey,
            RunInfoId = processInstanceId,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            NodeId = nodeId,
            ActionType = actionType,
        };
        _repository.Add(entrust);
    }
}
