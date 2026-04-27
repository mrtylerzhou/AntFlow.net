using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataService : IAntFlowRepositoryMix<BpmnConfLfFormdata, IBpmnConfLfFormdataRepository>
{
    List<BpmnConfLfFormdata> ListByConfId(long confId);
}
