using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnConfLfFormdataService
{
    List<BpmnConfLfFormdata> ListByConfId(long confId);
}