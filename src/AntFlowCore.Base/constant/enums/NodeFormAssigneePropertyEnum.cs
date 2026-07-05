namespace AntFlowCore.Base.constant.enums;

/// <summary>
/// Form assignee property type enum (aligned with Java NodeFormAssigneePropertyEnum).
/// </summary>
public enum NodeFormAssigneePropertyEnum
{
    FORM_ASSIGNEE = 1,
    FORM_ROLE = 2,
    FORM_USER_HRBP = 3,
    FORM_USER_DIRECT_LEADER = 4,
    FORM_USER_DEPART_LEADER = 5,
    FORM_DEPART_LEADER = 6,
    FORM_USER_LEVEL_LEADER = 7,
    FORM_USER_LOOP_LEADER = 8
}

public static class NodeFormAssigneePropertyEnumExtensions
{
    public static string GetDescByCode(int code)
    {
        return code switch
        {
            1 => "表单中的人员",
            2 => "表单中的角色",
            3 => "表单中人员的HRBP",
            4 => "表单中人员的直属领导",
            5 => "表单中人员所在部门负责人",
            6 => "表单中部门的负责人",
            7 => "表单中人员多级领导",
            8 => "表单中人员全部层级领导",
            _ => ""
        };
    }

    public static NodeFormAssigneePropertyEnum? GetByCode(int? code)
    {
        if (code == null)
        {
            return null;
        }
        return Enum.IsDefined(typeof(NodeFormAssigneePropertyEnum), code.Value)
            ? (NodeFormAssigneePropertyEnum)code.Value
            : null;
    }
}
