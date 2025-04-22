using antflowcore.constant.enums;
using antflowcore.vo;

namespace antflowcore.adaptor.bpmnnodeadp;

public class NodePropertyDirectLeaderAdaptor : BpmnNodeAdaptor
{
    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        return null;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_DIRECT_LEADER);
    }
}