using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service;

public interface IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo);
}