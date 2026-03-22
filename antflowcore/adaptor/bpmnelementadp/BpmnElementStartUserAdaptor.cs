using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 发起人自己审批元素适配器
/// </summary>
public class BpmnElementStartUserAdaptor : AbstractCommonBpmnElementSingleAdaptor
{
    protected override string CollectionName => "startUser";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_START_USER;
}
