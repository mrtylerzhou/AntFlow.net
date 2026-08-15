namespace AntFlowCore.Base.vo;

/// <summary>
/// 演示数据-人员管理 列表 VO. 对应 Java DemoDataUserVo.
/// </summary>
public class DemoDataUserVo
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
    /// 部门ID
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 直属领导ID
    /// </summary>
    public long? LeaderId { get; set; }

    /// <summary>
    /// 直属领导姓名
    /// </summary>
    public string? LeaderName { get; set; }

    /// <summary>
    /// HRBP ID
    /// </summary>
    public long? HrbpId { get; set; }

    /// <summary>
    /// HRBP 姓名
    /// </summary>
    public string? HrbpName { get; set; }

    /// <summary>
    /// 是否删除:0正常 1删除(与 Java 版返回一致)
    /// </summary>
    public int IsDel { get; set; }
}

