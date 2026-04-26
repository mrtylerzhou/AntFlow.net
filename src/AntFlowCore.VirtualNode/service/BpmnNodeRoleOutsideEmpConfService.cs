using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeRoleOutsideEmpConfService : IBpmnNodeRoleOutsideEmpConfService
{
    public BpmnNodeRoleOutsideEmpConfService(IBpmnNodeRoleOutsideEmpConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeRoleOutsideEmpConfRepository _repository { get; }
}
