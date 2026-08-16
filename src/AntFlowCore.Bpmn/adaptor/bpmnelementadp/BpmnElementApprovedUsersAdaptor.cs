using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for "被审批人自己" nodes (nodeProperty = 15, NODE_PROPERTY_APPROVED_USERS).
/// Mirrors Java BpmnElementApprovedUsersAdaptor; varName "approvedUsers".
/// </summary>
public class BpmnElementApprovedUsersAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "approvedUsers";
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_APPROVED_USERS);
    }
}