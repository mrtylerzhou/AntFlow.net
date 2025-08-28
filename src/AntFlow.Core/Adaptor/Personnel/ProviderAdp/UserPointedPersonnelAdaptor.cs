using AntFlow.Core.Adaptor.Personnel.Provider;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Service;

namespace AntFlow.Core.Adaptor.Personnel;

public class UserPointedPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public UserPointedPersonnelAdaptor(UserPointedPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService,
        bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.USERAPPOINTED_PERSONNEL);
    }
}