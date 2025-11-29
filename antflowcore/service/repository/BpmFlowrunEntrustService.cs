using AntFlowCore.Entity;
using antflowcore.service.interf;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmFlowrunEntrustService : AFBaseCurdRepositoryService<BpmFlowrunEntrust>,IBpmFlowrunEntrustService
{
    public BpmFlowrunEntrustService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId)
    {
        BpmFlowrunEntrust bpmFlowrunEntrust = this.baseRepo.Where(a =>
            a.Actual.Equals(actual) && a.RunInfoId.Equals(procDefId) && a.RunTaskId.Equals(taskId)).First();
        return bpmFlowrunEntrust;
    }

    public void AddFlowrunEntrust(String actual, String actualName, String original, String originalName,
        String runtaskid, int type, String processInstanceId, String processKey,string nodeId,int actionType)
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
        this.baseRepo.Insert(entrust);
    }
}