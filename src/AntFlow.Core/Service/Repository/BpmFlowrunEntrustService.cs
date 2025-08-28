using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Util;

namespace AntFlow.Core.Service.Repository;

public class BpmFlowrunEntrustService : AFBaseCurdRepositoryService<BpmFlowrunEntrust>, IBpmFlowrunEntrustService
{
    public BpmFlowrunEntrustService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId)
    {
        BpmFlowrunEntrust bpmFlowrunEntrust = baseRepo.Where(a =>
            a.Actual.Equals(actual) && a.RunInfoId.Equals(procDefId) && a.RunTaskId.Equals(taskId)).First();
        return bpmFlowrunEntrust;
    }

    public void AddFlowrunEntrust(string actual, string actualName, string original, string originalName,
        string runtaskid, int type, string processInstanceId, string processKey)
    {
        BpmFlowrunEntrust? entrust = new()
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
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };
        baseRepo.Insert(entrust);
    }
}