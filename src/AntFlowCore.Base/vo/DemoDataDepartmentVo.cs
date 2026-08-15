namespace AntFlowCore.Base.vo;

/// <summary>
/// 演示数据-部门管理 列表 VO. 对应 Java DemoDataDepartmentVo.
/// </summary>
public class DemoDataDepartmentVo
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 简称
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// 上级部门ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 上级部门名称
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// 负责人ID
    /// </summary>
    public long? LeaderId { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    public string? LeaderName { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 是否删除:0正常 1删除(与 Java 版返回一致)
    /// </summary>
    public int IsDel { get; set; }
}

