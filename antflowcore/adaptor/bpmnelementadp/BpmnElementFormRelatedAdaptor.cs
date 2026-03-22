using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 表单关联审批元素适配器
/// </summary>
public class BpmnElementFormRelatedAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "formUsers";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_FORM_RELATED;
}
