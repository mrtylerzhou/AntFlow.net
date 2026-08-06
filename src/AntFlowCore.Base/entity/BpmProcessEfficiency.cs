namespace AntFlowCore.Base.entity;

/// <summary>
/// 流程效能统计实体
/// </summary>
public class BpmProcessEfficiency
{
    public const int TYPE_TASK = 1;
    public const int TYPE_NODE = 2;
    public const int TYPE_PROCESS = 3;

    public long Id { get; set; }

    /// <summary>
    /// 流程类型编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 流程编号
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 流程实例ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    /// 执行实例ID
    /// </summary>
    public string ExecutionId { get; set; }

    /// <summary>
    /// 任务定义Key
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; }

    /// <summary>
    /// 审批人ID
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// 审批人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    /// 统计类型: 1=任务级, 2=节点级, 3=流程级
    /// </summary>
    public int StaticType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 耗时(毫秒)
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 流程状态
    /// </summary>
    public int? ProcessState { get; set; }

    /// <summary>
    /// 流程创建时间
    /// </summary>
    public DateTime? ProcessCreateTime { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; } = DateTime.Now;
}
