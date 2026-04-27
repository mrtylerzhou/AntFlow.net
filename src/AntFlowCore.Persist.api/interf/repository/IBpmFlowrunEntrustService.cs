using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmFlowrunEntrustService : IAntFlowRepositoryMix<BpmFlowrunEntrust, IBpmFlowrunEntrustRepository>
{
    BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId);

    void AddFlowrunEntrust(String actual, String actualName, String original, String originalName,
        String runtaskid, int type, String processInstanceId, String processKey, string nodeId, int actionType);
}
