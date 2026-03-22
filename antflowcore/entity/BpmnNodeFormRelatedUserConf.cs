namespace AntFlowCore.Entity;

/// <summary>
/// 表单关联用户配置实体
/// </summary>
public class BpmnNodeFormRelatedUserConf
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 节点ID
    /// </summary>
    public long BpmnNodeId { get; set; }

    /// <summary>
    /// 值JSON数组
    /// </summary>
    public string ValueJson { get; set; }

    /// <summary>
    /// 签批类型 1:会签 2:或签
    /// </summary>
    public int? SignType { get; set; }

    /// <summary>
    /// 值类型
    /// </summary>
    public int? ValueType { get; set; }

    /// <summary>
    /// 值类型名称
    /// </summary>
    public string ValueTypeName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 是否删除 0:正常 1:删除
    /// </summary>
    public int? IsDel { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
