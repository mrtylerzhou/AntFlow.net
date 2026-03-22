using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 用户自定义规则BPMN元素适配器
/// </summary>
public class BpmnElementUDRAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "udr";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_ZDY_RULES;
}
