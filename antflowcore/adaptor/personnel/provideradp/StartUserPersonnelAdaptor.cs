using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enus;
using antflowcore.service;

namespace antflowcore.adaptor.personnel;

public class StartUserPersonnelAdaptor: AbstractBpmnPersonnelAdaptor
{
    public StartUserPersonnelAdaptor(StartUserPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.START_USER_PERSONNEL);
    }
}