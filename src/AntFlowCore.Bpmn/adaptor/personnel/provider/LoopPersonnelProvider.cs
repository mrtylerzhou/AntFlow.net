using AntFlowCore.Abstraction;
using AntFlowCore.AspNetCore.AspNetCore.conf.di;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
[NamedService(nameof(LoopPersonnelProvider))]
public class LoopPersonnelProvider : IBpmnPersonnelProviderService {

public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo) {
    return null;
}
}
