using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service;

namespace antflowcore.adaptor.personnel.provideradp;

public class DirectLeaderPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public DirectLeaderPersonnelAdaptor(DirectLeaderPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.DIRECT_LEADER_PERSONNEL);
    }
}