using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNodeSubmitService : IAntFlowRepositoryMix<BpmProcessNodeSubmit, IBpmProcessNodeSubmitRepository>
{
    BpmProcessNodeSubmit? FindBpmProcessNodeSubmit(string processInstanceId);
    bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit);
}
