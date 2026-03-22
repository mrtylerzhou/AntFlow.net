using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 指定角色审批元素适配器
/// </summary>
public class BpmnElementRoleAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "roleUserList";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_ROLE;
}
