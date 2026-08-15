namespace AntFlowCore.Base.dto;

/// <summary>
/// 演示数据-人员/部门/角色管理 列表分页请求. 对应 Java DemoDataMgmtPageReq.
/// 通用请求体:不同列表按需使用对应字段.
/// </summary>
public class DemoDataMgmtPageReq
{
    public PageDto? PageDto { get; set; }

    /// <summary>
    /// 人员管理:姓名(模糊)
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 人员管理:手机号(模糊)
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// 部门管理:部门名称(模糊)
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 角色管理:角色名称(模糊)
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色详情:角色ID(查看角色下人员)
    /// </summary>
    public long? RoleId { get; set; }
}
