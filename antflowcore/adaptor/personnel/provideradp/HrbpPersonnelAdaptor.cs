using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service;

namespace antflowcore.adaptor.personnel.provideradp;

public class HrbpPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public HrbpPersonnelAdaptor(HrbpPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.HRBP_PERSONNEL);
    }
}