using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnViewPageButtonService : IAntFlowRepositoryMix<BpmnViewPageButton, IBpmnViewPageButtonRepository>
{
    void DeleteByConfId(long confId);
}
