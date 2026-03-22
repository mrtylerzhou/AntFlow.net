namespace AntFlowCore.Entity;

/// <summary>
/// 用户自定义规则节点配置实体
/// </summary>
public class BpmnNodeUDRConf
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
    /// 自定义规则属性标识（对应字典中的value）
    /// </summary>
    public string UdrProperty { get; set; }

    /// <summary>
    /// 自定义规则属性名称
    /// </summary>
    public string UdrPropertyName { get; set; }

    /// <summary>
    /// 自定义扩展字段1
    /// </summary>
    public string Ext1 { get; set; }

    /// <summary>
    /// 自定义扩展字段2
    /// </summary>
    public string Ext2 { get; set; }

    /// <summary>
    /// 自定义扩展字段3
    /// </summary>
    public string Ext3 { get; set; }

    /// <summary>
    /// 自定义扩展字段4
    /// </summary>
    public string Ext4 { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 是否删除 0:正常 1:删除
    /// </summary>
    public int IsDel { get; set; }

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
