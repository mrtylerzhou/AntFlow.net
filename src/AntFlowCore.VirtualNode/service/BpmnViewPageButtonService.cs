using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnViewPageButtonService : IBpmnViewPageButtonService
{
    public BpmnViewPageButtonService(IBpmnViewPageButtonRepository repository)
    {
        _repository = repository;
    }

    public IBpmnViewPageButtonRepository _repository { get; }

    public void DeleteByConfId(long confId)
    {
        _repository.DeleteByConfId(confId);
    }
}
