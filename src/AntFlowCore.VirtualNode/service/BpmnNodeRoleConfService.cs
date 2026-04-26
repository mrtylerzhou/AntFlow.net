using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeRoleConfService : IBpmnNodeRoleConfService
{
    public BpmnNodeRoleConfService(IBpmnNodeRoleConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeRoleConfRepository _repository { get; }
}
