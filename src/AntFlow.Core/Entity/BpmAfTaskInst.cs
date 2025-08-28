namespace AntFlow.Core.Entity;

/// <summary>
///     流程任务实例实体类
/// </summary>
public class BpmAfTaskInst
{
    /// <summary>
    ///     主键
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     流程定义 ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     任务定义键
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    ///     流程实例 ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    ///     执行实例 ID
    /// </summary>
    public string ExecutionId { get; set; }

    /// <summary>
    ///     任务名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     父任务 ID
    /// </summary>
    public string ParentTaskId { get; set; }

    /// <summary>
    ///     任务所有者
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    ///     任务受托人
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     受托人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    ///     原始处理人
    /// </summary>
    public string OriginalAssignee { get; set; }

    /// <summary>
    ///     原始处理人姓名
    /// </summary>
    public string OriginalAssigneeName { get; set; }

    /// <summary>
    ///     处理人变更原因
    /// </summary>
    public string TransferReason { get; set; }

    /// <summary>
    ///     审批状态
    /// </summary>
    public int VerifyStatus { get; set; }

    /// <summary>
    ///     审核意见
    /// </summary>
    public string VerifyDesc { get; set; }

    /// <summary>
    ///     任务开始时间
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>
    ///     任务认领时间
    /// </summary>
    public DateTime? ClaimTime { get; set; }

    /// <summary>
    ///     任务结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     持续时间（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    ///     删除原因
    /// </summary>
    public string DeleteReason { get; set; }

    /// <summary>
    ///     优先级
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    ///     到期时间
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    ///     表单标识
    /// </summary>
    public string FormKey { get; set; }

    /// <summary>
    ///     类别
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    ///     租户 ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    ///     描述信息
    /// </summary>
    public string Description { get; set; }

    public string UpdateUser { get; set; }
}