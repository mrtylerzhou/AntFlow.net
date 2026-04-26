using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodePersonnelEmplConfService : IBpmnNodePersonnelEmplConfService
{
    public BpmnNodePersonnelEmplConfService(IBpmnNodePersonnelEmplConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodePersonnelEmplConfRepository _repository { get; }
}
