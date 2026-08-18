using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for role nodes.
/// </summary>
public class BpmnElementRoleAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "roleUserList";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_ROLE);
    }
}