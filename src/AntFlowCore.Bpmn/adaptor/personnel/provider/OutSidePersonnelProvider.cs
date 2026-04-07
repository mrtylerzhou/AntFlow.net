using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service;
using AntFlowCore.AspNetCore.AspNetCore.conf.di;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
[NamedService(nameof(OutSidePersonnelProvider))]
public class OutSidePersonnelProvider: IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        return null;
    }
}