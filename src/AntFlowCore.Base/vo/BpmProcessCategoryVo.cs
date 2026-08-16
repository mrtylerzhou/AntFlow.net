namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程分类 VO. 对应 Java BpmProcessCategoryVo.
/// </summary>
public class BpmProcessCategoryVo
{
    public long? Id { get; set; }

    /// <summary>
    /// process type name
    /// </summary>
    public string? ProcessTypeName { get; set; }

    public int? IsDel { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// is for app 0:no 1:yes
    /// </summary>
    public int? IsApp { get; set; }

    public int? State { get; set; }

    /// <summary>
    /// entrance PC/APP
    /// </summary>
    public string? Entrance { get; set; }

    public int? Type { get; set; }

    public string? Name { get; set; }
}
