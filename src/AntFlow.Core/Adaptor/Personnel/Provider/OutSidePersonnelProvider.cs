using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(OutSidePersonnelProvider))]
public class OutSidePersonnelProvider : IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        return null;
    }
}