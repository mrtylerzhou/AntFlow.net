using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// HRBP审批元素适配器
/// </summary>
public class BpmnElementHrbpAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "hrbpUser";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_HRBP;
}
