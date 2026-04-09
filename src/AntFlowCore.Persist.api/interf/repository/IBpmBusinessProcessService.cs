using AntFlowCore.Base.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmBusinessProcessService : IBaseRepositoryService<BpmBusinessProcess>
{
    BpmBusinessProcess GetBpmBusinessProcess(string processNumber);
    BpmBusinessProcess GetBpmBusinessProcessByProcInstId(string procinstId);
    void Update(BpmBusinessProcess bpmBusinessProcess);
    void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess);
    bool CheckProcessData(String entryId);
}
