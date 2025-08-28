using AntFlow.Core.Adaptor.Personnel.Provider;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Service;

namespace AntFlow.Core.Adaptor.Personnel.BusinessTableAdp;

public class BusinessTablePersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public BusinessTablePersonnelAdaptor(BusinessTablePersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService,
        bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.BUSINESS_TABLE_PERSONNEL);
    }
}