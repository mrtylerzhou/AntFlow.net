using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for business-table nodes. Mirrors Java BpmnElementBusinessTableAdp (varName "businessList").
/// </summary>
public class BpmnElementBusinessTableAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "businessList";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE);
    }
}