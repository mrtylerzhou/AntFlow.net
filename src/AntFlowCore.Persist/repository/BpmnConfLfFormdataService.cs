using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfLfFormdataService : IBpmnConfLfFormdataService
{
    public BpmnConfLfFormdataService(IBpmnConfLfFormdataRepository repository)
    {
        _repository = repository;
    }

    public IBpmnConfLfFormdataRepository _repository { get; }

    public List<BpmnConfLfFormdata> ListByConfId(long confId)
    {
        List<BpmnConfLfFormdata> lfFormdatas = _repository.Find(a => a.BpmnConfId == confId);
        return lfFormdatas;
    }
}
