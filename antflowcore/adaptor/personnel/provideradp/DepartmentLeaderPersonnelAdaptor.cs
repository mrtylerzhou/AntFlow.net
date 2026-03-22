using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.service;
using antflowcore.service.interf;

namespace antflowcore.adaptor.personnel.provideradp;

/// <summary>
/// 部门负责人人员适配器
/// </summary>
public class DepartmentLeaderPersonnelAdaptor : AbstractBpmnPersonnelAdaptor
{
    public DepartmentLeaderPersonnelAdaptor(
        DepartmentLeaderPersonnelProvider bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService) : base(bpmnPersonnelProviderService, bpmnEmployeeInfoProviderService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(PersonnelEnum.DEPARTMENT_LEADER_PERSONNEL);
    }
}
