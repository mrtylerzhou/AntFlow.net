namespace AntFlowCore.Base.vo;

/// <summary>
/// 演示数据-角色详情(角色下人员) VO. 对应 Java DemoDataRoleUserVo.
/// </summary>
public class DemoDataRoleUserVo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DepartmentName { get; set; }
}
