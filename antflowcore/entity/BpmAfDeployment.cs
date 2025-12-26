using AntFlowCore.Constants;

namespace antflowcore.entity;

/// <summary>
/// BpmAfDeployment Entity
/// </summary>
public class BpmAfDeployment
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 修订号
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    /// 部署名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 部署内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDel { get; set; }
    public string TenantId { get; set; }
    /// <summary>
    /// 创建用户
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新用户
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}