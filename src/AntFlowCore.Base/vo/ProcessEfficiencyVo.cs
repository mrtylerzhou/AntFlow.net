using AntFlowCore.Base.dto;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程效能统计查询VO
/// </summary>
public class ProcessEfficiencyVo
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public PageDto PageDto { get; set; }

    /// <summary>
    /// 流程类型编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 流程编号
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 审批人(姓名或ID模糊匹配)
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// 流程状态
    /// </summary>
    public int? ProcessState { get; set; }

    /// <summary>
    /// 开始时间范围-起
    /// </summary>
    public DateTime? StartTimeBegin { get; set; }

    /// <summary>
    /// 开始时间范围-止
    /// </summary>
    public DateTime? StartTimeEnd { get; set; }

    /// <summary>
    /// 流程实例ID(展开节点级时使用)
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    /// 任务定义Key(展开任务级时使用)
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 统计计算接口参数:formCode列表
    /// </summary>
    public List<string> FormCodes { get; set; }
}
