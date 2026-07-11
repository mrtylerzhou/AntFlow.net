using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for form-related approvers (NODE_PROPERTY_FORM_RELATED).
/// The approver is chosen from the form; the variable name is "formUsers".
/// Mirrors the Java BpmnElementFormRelatedAdp.
/// </summary>
public class BpmnElementFormRelatedAdp : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "formUsers";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_FORM_RELATED);
    }
}
