using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for user-defined-rule approvers (NODE_PROPERTY_ZDY_RULES).
/// The approver is resolved by a custom rule; the variable name is "udrUsers".
/// Mirrors the Java BpmnElementUDRAdp.
/// </summary>
public class BpmnElementUDRAdp : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "udrUsers";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_ZDY_RULES);
    }
}
