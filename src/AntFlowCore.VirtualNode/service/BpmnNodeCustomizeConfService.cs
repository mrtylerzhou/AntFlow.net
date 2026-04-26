using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeCustomizeConfService : IBpmnNodeCustomizeConfService
{
    public BpmnNodeCustomizeConfService(IBpmnNodeCustomizeConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeCustomizeConfRepository _repository { get; }
}
