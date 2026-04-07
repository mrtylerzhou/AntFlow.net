


using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmFlowrunEntrustService : IBaseRepositoryService<BpmFlowrunEntrust>
{
    BpmFlowrunEntrust GetEntrustByTaskId(string actual, string procDefId, string taskId);

    void AddFlowrunEntrust(String actual, String actualName, String original, String originalName,
        String runtaskid, int type, String processInstanceId, String processKey,string nodeId,int actionType);
}
