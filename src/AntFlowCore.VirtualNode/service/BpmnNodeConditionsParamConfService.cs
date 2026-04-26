using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeConditionsParamConfService : IBpmnNodeConditionsParamConfService
{
    public BpmnNodeConditionsParamConfService(IBpmnNodeConditionsParamConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeConditionsParamConfRepository _repository { get; }
}
