using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnViewPageButtonService : IAntFlowRepositoryMix<BpmnViewPageButton, IBpmnViewPageButtonRepository>
{
    void DeleteByConfId(long confId);
}
