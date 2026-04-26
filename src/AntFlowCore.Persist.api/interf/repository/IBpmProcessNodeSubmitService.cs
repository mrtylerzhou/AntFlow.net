using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNodeSubmitService : IAntFlowRepositoryMix<BpmProcessNodeSubmit, IBpmProcessNodeSubmitRepository>
{
    BpmProcessNodeSubmit? FindBpmProcessNodeSubmit(string processInstanceId);
    bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit);
}
