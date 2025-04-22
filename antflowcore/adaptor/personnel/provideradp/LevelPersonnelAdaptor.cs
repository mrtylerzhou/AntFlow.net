using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service;

namespace antflowcore.adaptor.personnel.provideradp;

public class LevelPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public LevelPersonnelAdaptor(LevelPersonnelProvider bpmnPersonnelProviderService, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.NODE_LEVEL_PERSONNEL);
    }
}