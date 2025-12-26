using AntFlowCore.Constants;

namespace antflowcore.entity;

/// <summary>
/// Represents a BPM variable multiplayer.
/// </summary>
public class BpmVariableMultiplayer
{
    /// <summary>
    /// 自增ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 变量ID
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    /// 元素ID
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// 节点ID
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    /// 元素名称
    /// </summary>
    public string ElementName { get; set; }

    /// <summary>
    /// 集合名称
    /// </summary>
    public string CollectionName { get; set; }

    /// <summary>
    /// 签名类型 1表示全签，2表示或签
    /// </summary>
    public int SignType { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    /// <summary>
    /// 是否删除，0表示否，1表示是
    /// </summary>
    public int IsDel { get; set; }
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

    /// <summary>
    /// 处理状态，忽略映射
    /// </summary>
    public int? UnderTakeStatus { get; set; }
}