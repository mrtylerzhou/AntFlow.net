using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程千里眼 - 请求VO
/// </summary>
public class FlowClairvoyanceVo
{
    /// <summary>
    /// 目标审批人ID列表(1~5个, OR关系)
    /// </summary>
    [JsonPropertyName("userIds")]
    public List<string> UserIds { get; set; }

    /// <summary>
    /// 时间范围(天数): 1/3/5/7/15/30/180
    /// </summary>
    [JsonPropertyName("timeRange")]
    public int? TimeRange { get; set; }

    /// <summary>
    /// 节点范围: CURRENT / FUTURE / CURRENT_FUTURE / ALL
    /// </summary>
    [JsonPropertyName("nodeScope")]
    public string NodeScope { get; set; }

    /// <summary>
    /// 内部扫描偏移量(前端维护, 首次传0)
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}
