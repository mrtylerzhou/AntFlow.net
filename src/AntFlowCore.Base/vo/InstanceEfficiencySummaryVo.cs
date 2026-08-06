namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程实例效能-顶部汇总 VO
/// </summary>
public class InstanceEfficiencySummaryVo
{
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 流程状态
    /// </summary>
    public int? ProcessState { get; set; }

    public string ProcessStateName { get; set; }

    /// <summary>
    /// 流程发起时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 当时耗时(毫秒)
    /// 进行中: now - createTime
    /// 已完成: max(节点end_time) - createTime
    /// </summary>
    public long? TotalDuration { get; set; }

    /// <summary>
    /// 当时耗时(格式化文本)
    /// </summary>
    public string TotalDurationText { get; set; }

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool Finished { get; set; }
}
