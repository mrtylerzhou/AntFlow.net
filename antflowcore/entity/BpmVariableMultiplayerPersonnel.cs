using AntFlowCore.Constants;

namespace antflowcore.entity;

/// <summary>
/// 变量多方人员
/// </summary>
public class BpmVariableMultiplayerPersonnel
{
    /// <summary>
    /// 自增ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 变量多方ID
    /// </summary>
    public long VariableMultiplayerId { get; set; }

    /// <summary>
    /// 被指派人
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// 被指派人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    /// 是否承担，0表示否，1表示是
    /// </summary>
    public int? UndertakeStatus { get; set; }

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
}