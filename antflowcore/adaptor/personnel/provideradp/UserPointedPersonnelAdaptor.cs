using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enus;
using antflowcore.service;

namespace antflowcore.adaptor.personnel;

public class UserPointedPersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public UserPointedPersonnelAdaptor(UserPointedPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.USERAPPOINTED_PERSONNEL);
    }
}