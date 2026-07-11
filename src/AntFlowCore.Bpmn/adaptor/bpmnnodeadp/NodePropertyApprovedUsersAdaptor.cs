using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// Node adaptor for "被审批人自己" nodes (nodeProperty = 15, NODE_PROPERTY_APPROVED_USERS).
/// Mirrors Java NodePropertyApprovedUsersAdp: extends AbstractAdditionSignNodeAdaptor
/// and sets DeduplicationExclude = true so approved users are excluded from deduplication.
/// </summary>
public class NodePropertyApprovedUsersAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyApprovedUsersAdaptor(IRoleService roleService) : base(roleService)
    {
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);
        bpmnNodeVo.DeduplicationExclude = true;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_APPROVED_USERS);
    }
}
