using AntFlowCore.Entity;

namespace antflowcore.service.interf;

public interface IBpmFlowrunEntrustService
{
    BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId);

    void AddFlowrunEntrust(String actual, String actualName, String original, String originalName,
        String runtaskid, int type, String processInstanceId, String processKey);
}