using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmBusinessProcessService : IAntFlowRepositoryMix<BpmBusinessProcess,IBpmBusinessProcessRepository>
{
    BpmBusinessProcess GetBpmBusinessProcess(string processNumber);
    BpmBusinessProcess GetBpmBusinessProcessByProcInstId(string procinstId);
    void Update(BpmBusinessProcess bpmBusinessProcess);
    void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess);
    bool CheckProcessData(String entryId);
}
