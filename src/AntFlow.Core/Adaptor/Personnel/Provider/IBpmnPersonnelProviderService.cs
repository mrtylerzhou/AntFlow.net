using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

public interface IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo);
}