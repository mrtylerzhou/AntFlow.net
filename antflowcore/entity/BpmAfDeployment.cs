using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// BpmAfDeployment Entity
/// </summary>
[Table(Name = "bpm_af_deployment")]
public class BpmAfDeployment
{
    /// <summary>
    /// 主键
    /// </summary>
    [Column(Name = "id", IsPrimary = true)]
    public string Id { get; set; }

    /// <summary>
    /// 修订号
    /// </summary>
    [Column(Name = "rev")]
    public int? Rev { get; set; }

    /// <summary>
    /// 部署名称
    /// </summary>
    [Column(Name = "name")]
    public string Name { get; set; }

    /// <summary>
    /// 部署内容
    /// </summary>
    [Column(Name = "content")]
    public string Content { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(Name = "remark")]
    public string Remark { get; set; } = "";

    /// <summary>
    /// 是否删除
    /// </summary>
    [Column(Name = "is_del")]
    public bool IsDel { get; set; }

    /// <summary>
    /// 创建用户
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "create_time")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新用户
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }
}