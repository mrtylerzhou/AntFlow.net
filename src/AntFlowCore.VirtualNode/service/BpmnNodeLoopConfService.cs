using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeLoopConfService : IBpmnNodeLoopConfService
{
    public BpmnNodeLoopConfService(IBpmnNodeLoopConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeLoopConfRepository _repository { get; }
}
