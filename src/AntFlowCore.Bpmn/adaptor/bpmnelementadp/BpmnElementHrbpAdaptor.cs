using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for HRBP nodes (single assignee). Mirrors Java BpmnElementHrbpAdp (varName "hrbpUser").
/// </summary>
public class BpmnElementHrbpAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "hrbpUser";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_HRBP);
    }
}