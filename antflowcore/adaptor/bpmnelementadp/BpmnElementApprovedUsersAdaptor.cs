using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 被审批人自己BPMN元素适配器
/// </summary>
public class BpmnElementApprovedUsersAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "approvedUsers";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_APPROVED_USERS;
}
