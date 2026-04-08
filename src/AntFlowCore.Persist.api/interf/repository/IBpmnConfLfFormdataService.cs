using AntFlowCore.Core.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataService : IBaseRepositoryService<BpmnConfLfFormdata>
{
    List<BpmnConfLfFormdata> ListByConfId(long confId);
}
