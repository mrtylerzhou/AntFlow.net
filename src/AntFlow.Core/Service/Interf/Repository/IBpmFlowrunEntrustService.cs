using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Interface;

public interface IBpmFlowrunEntrustService
{
    BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId);

    void AddFlowrunEntrust(string actual, string actualName, string original, string originalName,
        string runtaskid, int type, string processInstanceId, string processKey);
}