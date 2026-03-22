using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.service;
using antflowcore.service.interf;

namespace antflowcore.adaptor.personnel.provideradp;

/// <summary>
/// 被审批人自己人员适配器
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
