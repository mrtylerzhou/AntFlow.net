using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for previous-node-related user nodes (nodeProperty = 18, NODE_PROPERTY_PREV_NODE_RELATED).
/// Mirrors Java BpmnElementPrevNodeAdp (varName "prevNodeUsers").
/// </summary>
public class BpmnElementPrevNodeAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "prevNodeUsers";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_PREV_NODE_RELATED);
    }
}