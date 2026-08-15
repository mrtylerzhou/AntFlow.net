namespace AntFlowCore.Base.vo;

/// <summary>
/// 演示数据-角色管理 列表 VO. 对应 Java DemoDataRoleVo.
/// </summary>
public class DemoDataRoleVo
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色下关联人员数量
    /// </summary>
    public long UserCount { get; set; }
}
