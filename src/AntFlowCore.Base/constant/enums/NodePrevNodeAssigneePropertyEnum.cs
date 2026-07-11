namespace AntFlowCore.Base.constant.enums;

/// <summary>
/// Previous node assignee property type enum (aligned with Java NodePrevNodeAssigneePropertyEnum).
/// Mirrors NodeFormAssigneePropertyEnum but the first item is "上一节点人员" instead of "表单中的人员".
/// </summary>
public enum NodePrevNodeAssigneePropertyEnum
{
    PREV_NODE_ASSIGNEE = 1,
    PREV_NODE_ROLE = 2,
    PREV_NODE_USER_HRBP = 3,
    PREV_NODE_USER_DIRECT_LEADER = 4,
    PREV_NODE_USER_DEPART_LEADER = 5,
    PREV_NODE_DEPART_LEADER = 6,
    PREV_NODE_USER_LEVEL_LEADER = 7,
    PREV_NODE_USER_LOOP_LEADER = 8
}

public static class NodePrevNodeAssigneePropertyEnumExtensions
{
    public static string GetDescByCode(int code)
    {
        return code switch
        {
            1 => "上一节点人员",
            2 => "上一节点人员的角色",
            3 => "上一节点人员的HRBP",
            4 => "上一节点人员的直属领导",
            5 => "上一节点人员所在部门负责人",
            6 => "上一节点部门的负责人",
            7 => "上一节点人员多级领导",
            8 => "上一节点人员全部层级领导",
            _ => ""
        };
    }

    public static NodePrevNodeAssigneePropertyEnum? GetByCode(int? code)
    {
        if (code == null)
        {
            return null;
        }
        return Enum.IsDefined(typeof(NodePrevNodeAssigneePropertyEnum), code.Value)
            ? (NodePrevNodeAssigneePropertyEnum)code.Value
            : null;
    }
}
