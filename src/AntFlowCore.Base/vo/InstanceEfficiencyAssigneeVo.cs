namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程实例效能-人员明细 VO
/// </summary>
public class InstanceEfficiencyAssigneeVo
{
    /// <summary>
    /// 审批人 ID
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// 审批人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    /// 任务开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 任务结束时间(null=未完成)
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 个人耗时(毫秒)
    /// 已完成:取 af_hi_taskinst.duration
    /// 未完成:now - start_time
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 个人耗时(格式化文本)
    /// </summary>
    public string DurationText { get; set; }

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool Finished { get; set; }
}
