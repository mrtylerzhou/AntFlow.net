using System;

namespace antflowcore.entity;

/// <summary>
/// 流程草稿实体
/// </summary>
public class BpmBusinessDraft
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程编码
    /// </summary>
    public string BpmnCode { get; set; }

    /// <summary>
    /// 流程Key（表单编码）
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// 流程编号
    /// </summary>
    public string ProcessCode { get; set; }

    /// <summary>
    /// 流程节点
    /// </summary>
    public string ProcessNode { get; set; }

    /// <summary>
    /// 草稿JSON数据
    /// </summary>
    public string DraftJson { get; set; }

    /// <summary>
    /// 创建用户ID
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建用户名称
    /// </summary>
    public string CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 是否删除：0=否，1=是
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }
}
