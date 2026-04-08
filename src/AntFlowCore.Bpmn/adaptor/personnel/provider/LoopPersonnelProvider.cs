using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
[NamedService(nameof(LoopPersonnelProvider))]
public class LoopPersonnelProvider : IBpmnPersonnelProviderService {

public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo) {
    return null;
}
}
