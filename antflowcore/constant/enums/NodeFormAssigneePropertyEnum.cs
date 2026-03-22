namespace AntFlowCore.Constant.Enums;

/// <summary>
/// 表单关联人员属性枚举
/// </summary>
public class NodeFormAssigneeProperty
{
    public int Code { get; }
    public string Desc { get; }

    private NodeFormAssigneeProperty(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    /// <summary>
    /// 表单中的人员
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormAssignee = new NodeFormAssigneeProperty(1, "表单中的人员");

    /// <summary>
    /// 表单中的角色
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormRole = new NodeFormAssigneeProperty(2, "表单中的角色");

    /// <summary>
    /// 表单中人员的HRBP
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormUserHrbp = new NodeFormAssigneeProperty(3, "表单中人员的HRBP");

    /// <summary>
    /// 表单中人员的直属领导
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormUserDirectLeader = new NodeFormAssigneeProperty(4, "表单中人员的直属领导");

    /// <summary>
    /// 表单中人员所在部门负责人
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormUserDepartLeader = new NodeFormAssigneeProperty(5, "表单中人员所在部门负责人");

    /// <summary>
    /// 表单中部门的负责人
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormDepartLeader = new NodeFormAssigneeProperty(6, "表单中部门的负责人");

    /// <summary>
    /// 表单中人员多级领导
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormUserLevelLeader = new NodeFormAssigneeProperty(7, "表单中人员多级领导");

    /// <summary>
    /// 表单中人员全部层级领导
    /// </summary>
    public static readonly NodeFormAssigneeProperty FormUserLoopLeader = new NodeFormAssigneeProperty(8, "表单中人员全部层级领导");

    /// <summary>
    /// 获取所有枚举值
    /// </summary>
    public static readonly IReadOnlyList<NodeFormAssigneeProperty> AllValues = new List<NodeFormAssigneeProperty>
    {
        FormAssignee,
        FormRole,
        FormUserHrbp,
        FormUserDirectLeader,
        FormUserDepartLeader,
        FormDepartLeader,
        FormUserLevelLeader,
        FormUserLoopLeader
    };

    /// <summary>
    /// 根据code获取枚举
    /// </summary>
    public static NodeFormAssigneeProperty GetByCode(int? code)
    {
        if (code == null) return null;
        foreach (var value in AllValues)
        {
            if (value.Code == code) return value;
        }
        return null;
    }

    /// <summary>
    /// 根据code获取描述
    /// </summary>
    public static string GetDescByCode(int? code)
    {
        if (code == null) return "";
        var property = GetByCode(code);
        return property?.Desc ?? "";
    }
}
