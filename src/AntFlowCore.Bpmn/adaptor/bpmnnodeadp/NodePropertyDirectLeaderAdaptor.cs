using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyDirectLeaderAdaptor: AbstractAdditionSignNodeAdaptor
{
    public NodePropertyDirectLeaderAdaptor(IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
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