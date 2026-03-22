using antflowcore.constant.enus;

namespace antflowcore.adaptor.bpmnelementadp;

/// <summary>
/// 部门负责人BPMN元素适配器
/// </summary>
public class BpmnElementDepartmentLeaderAdaptor : AbstractCommonBpmnElementMultiAdaptor
{
    protected override string CollectionName => "departmentLeader";
    protected override NodePropertyEnum SupportedNodeProperty => NodePropertyEnum.NODE_PROPERTY_DEPARTMENT_LEADER;
}
