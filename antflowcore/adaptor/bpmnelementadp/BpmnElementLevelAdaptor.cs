using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 指定层级审批元素适配器
/// </summary>
public class BpmnElementLevelAdaptor : AbstractCommonBpmnElementSingleAdaptor
{
    protected override string CollectionName => "levelUser";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_LEVEL;
}
