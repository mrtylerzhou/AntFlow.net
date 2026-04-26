using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeAssignLevelConfService : IBpmnNodeAssignLevelConfService
{
    public BpmnNodeAssignLevelConfService(IBpmnNodeAssignLevelConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeAssignLevelConfRepository _repository { get; }
}
