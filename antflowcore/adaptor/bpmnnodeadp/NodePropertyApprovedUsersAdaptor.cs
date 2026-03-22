using antflowcore.constant.enus;
using antflowcore.service.interf.repository;
using antflowcore.vo;

namespace antflowcore.adaptor.bpmnnodeadp;

/// <summary>
/// 被审批人自己节点属性适配器
/// 被审批人不需要去重，因此设置DeduplicationExclude=true
/// </summary>
public class NodePropertyApprovedUsersAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyApprovedUsersAdaptor(
        service.repository.BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
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
