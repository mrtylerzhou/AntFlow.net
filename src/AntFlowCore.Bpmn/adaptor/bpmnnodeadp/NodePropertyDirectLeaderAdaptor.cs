using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.adaptor;
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