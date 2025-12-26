using AntFlowCore.Constants;

namespace antflowcore.entity;

using System;

/// <summary>
/// 流程变量审批提醒
/// </summary>
public class BpmVariableApproveRemind
{
    /// <summary>
    /// 自增主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程变量 ID
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    /// 流程元素 ID
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// 提醒内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    /// <summary>
    /// 删除标志
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