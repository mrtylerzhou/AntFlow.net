using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enus;
using antflowcore.service;

namespace antflowcore.adaptor.personnel;

public class RolePersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public RolePersonnelAdaptor(RolePersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.ROLE_PERSONNEL);
    }
}