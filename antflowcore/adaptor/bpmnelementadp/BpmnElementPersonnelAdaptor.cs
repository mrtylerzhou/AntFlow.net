using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 指定人员审批元素适配器
/// </summary>
public class BpmnElementPersonnelAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "personnelList";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
    protected override bool SupportSignInOrder => true;
}
