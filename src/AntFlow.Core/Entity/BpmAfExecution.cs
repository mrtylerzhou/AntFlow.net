namespace AntFlow.Core.Entity;

/// <summary>
///     流程执行实例实体类
/// </summary>
public class BpmAfExecution
{
    /// <summary>
    ///     主键
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     修订版本
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    ///     流程实例 ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    ///     业务键
    /// </summary>
    public string BusinessKey { get; set; }

    /// <summary>
    ///     父执行 ID
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    ///     流程定义 ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     超级执行 ID
    /// </summary>
    public string SuperExec { get; set; }

    /// <summary>
    ///     根流程实例 ID
    /// </summary>
    public string RootProcInstId { get; set; }

    /// <summary>
    ///     当前活动 ID
    /// </summary>
    public string ActId { get; set; }

    /// <summary>
    ///     是否激活
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    ///     是否并发
    /// </summary>
    public bool? IsConcurrent { get; set; }

    /// <summary>
    ///     租户 ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     启动用户 ID
    /// </summary>
    public string StartUserId { get; set; }

    /// <summary>
    ///     是否启用计数
    /// </summary>
    public bool? IsCountEnabled { get; set; }

    /// <summary>
    ///     事件订阅计数
    /// </summary>
    public int? EvtSubscrCount { get; set; }

    /// <summary>
    ///     任务计数
    /// </summary>
    public int? TaskCount { get; set; }

    /// <summary>
    ///     变量计数
    /// </summary>
    public int? VarCount { get; set; }

    /// <summary>
    ///     签收类型
    /// </summary>
    public int? SignType { get; set; }
}