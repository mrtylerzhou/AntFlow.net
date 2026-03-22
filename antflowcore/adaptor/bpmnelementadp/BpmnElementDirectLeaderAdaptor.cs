using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 直属领导审批元素适配器
/// </summary>
public class BpmnElementDirectLeaderAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "directLeader";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER;
}
