using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

public class DirectLeaderPersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public DirectLeaderPersonnelAdaptor(DirectLeaderPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.DIRECT_LEADER_PERSONNEL);
    }
}