using AntFlowCore.Entity;

namespace antflowcore.service.interf.repository;

public interface IBpmnConfLfFormdataService
{
    List<BpmnConfLfFormdata> ListByConfId(long confId);
}