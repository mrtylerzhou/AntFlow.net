using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodePersonnelConfService : IBpmnNodePersonnelConfService
{
    public BpmnNodePersonnelConfService(IBpmnNodePersonnelConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodePersonnelConfRepository _repository { get; }
}
