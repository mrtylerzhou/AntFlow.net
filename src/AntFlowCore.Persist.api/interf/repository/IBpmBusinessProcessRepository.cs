using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmBusinessProcessRepository: IBaseRepository<BpmBusinessProcess>
{
    void UpdateProcessDigest(string processNumber, string processDigest);
    void UpdateProcessState(long id, int processState);
    void UpdateProcInstId(long id, string procInstId);

    public void UpdateDto(BpmBusinessProcess bpmBusinessProcess);
}
