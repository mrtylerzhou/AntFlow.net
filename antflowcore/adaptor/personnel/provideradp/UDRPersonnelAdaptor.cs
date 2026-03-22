using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.service;
using antflowcore.service.interf;

namespace antflowcore.adaptor.personnel.provideradp;

/// <summary>
/// 用户自定义规则人员适配器
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
