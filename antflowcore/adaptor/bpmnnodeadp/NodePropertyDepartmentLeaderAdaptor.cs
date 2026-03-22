using antflowcore.constant.enus;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;

namespace antflowcore.adaptor.bpmnnodeadp;

/// <summary>
/// 部门负责人节点属性适配器
/// </summary>
public class NodePropertyDepartmentLeaderAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyDepartmentLeaderAdaptor(
       BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
       IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DEPARTMENT_LEADER);
    }
}
