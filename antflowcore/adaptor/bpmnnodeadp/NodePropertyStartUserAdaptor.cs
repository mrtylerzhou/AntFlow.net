using antflowcore.constant.enus;
using antflowcore.vo;

namespace antflowcore.adaptor;

public class NodePropertyStartUserAdaptor: BpmnNodeAdaptor
{
    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        bpmnNodeVo.DeduplicationExclude = true;
        return bpmnNodeVo;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
       
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_START_USER);
    }
}