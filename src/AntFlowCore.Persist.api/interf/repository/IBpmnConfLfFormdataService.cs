using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataService : IAntFlowRepositoryMix<BpmnConfLfFormdata, IBpmnConfLfFormdataRepository>
{
    List<BpmnConfLfFormdata> ListByConfId(long confId);
}
