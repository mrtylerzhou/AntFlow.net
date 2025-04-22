using antflowcore.conf.di;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;
[NamedService(nameof(OutSidePersonnelProvider))]
public class OutSidePersonnelProvider: IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        return null;
    }
}