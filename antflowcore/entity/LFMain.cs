namespace antflowcore.entity;

using FreeSql.DataAnnotations;
using System;

[Table(Name = "t_lf_main")]
public class LFMain
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = false)]
    public long Id { get; set; }

    /// <summary>
    /// 方便获取流程配置的额外字段
    /// </summary>
    [Column(Name = "conf_id")]
    public long? ConfId { get; set; }

    /// <summary>
    /// 方便获取表单编码的额外字段
    /// </summary>
    [Column(Name = "form_code")]
    public string FormCode { get; set; }

    /// <summary>
    /// 逻辑删除标记（0：未删除，1：已删除）
    /// </summary>
    [Column(Name = "is_del", IsNullable = false)]
    public int IsDel { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "create_time", CanUpdate = false)]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }
}