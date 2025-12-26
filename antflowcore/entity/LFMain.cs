namespace antflowcore.entity;

using System;

public class LFMain
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 方便获取流程配置的额外字段
    /// </summary>
    public long? ConfId { get; set; }

    /// <summary>
    /// 方便获取表单编码的额外字段
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 逻辑删除标记（0：未删除，1：已删除）
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    /// <summary>
    /// 创建人
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新人
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}