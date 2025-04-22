using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service;

namespace antflowcore.adaptor.personnel.provideradp;

public class RolePersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public RolePersonnelAdaptor(RolePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.ROLE_PERSONNEL);
    }
}