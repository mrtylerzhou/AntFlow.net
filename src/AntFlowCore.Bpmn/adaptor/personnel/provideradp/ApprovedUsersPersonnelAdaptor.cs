using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Bpmn.adaptor.personnel.provider;

namespace AntFlowCore.Bpmn.adaptor.personnel.provideradp;

/// <summary>
/// Personnel adaptor for "被审批人自己" nodes.
/// Maps PersonnelEnum.APPROVED_USERS_PERSONNEL to ApprovedUserPersonnelProvider.
/// </summary>
public class ApprovedUsersPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public ApprovedUsersPersonnelAdaptor(
        ApprovedUserPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.APPROVED_USERS_PERSONNEL);
    }
}
