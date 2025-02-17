using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enus;
using antflowcore.service;

namespace antflowcore.adaptor.personnel;

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