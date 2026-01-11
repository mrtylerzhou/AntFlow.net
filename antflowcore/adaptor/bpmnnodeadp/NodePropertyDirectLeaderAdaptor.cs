using antflowcore.constant.enus;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;

namespace antflowcore.adaptor;

public class NodePropertyDirectLeaderAdaptor: AbstractAdditionSignNodeAdaptor
{
    public NodePropertyDirectLeaderAdaptor(BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
        
    }
    public override void  FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        base.EditBpmnNode(bpmnNodeVo);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DIRECT_LEADER);
    }
}