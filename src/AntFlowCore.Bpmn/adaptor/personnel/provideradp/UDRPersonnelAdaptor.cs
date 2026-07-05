using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

/// <summary>
/// Personnel adaptor for user-defined-rule (UDR) nodes.
/// Maps PersonnelEnum.UDR_USERS_PERSONNEL to UDRPersonnelProvider.
/// </summary>
public class UDRPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public UDRPersonnelAdaptor(
        UDRPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.UDR_USERS_PERSONNEL);
    }
}
