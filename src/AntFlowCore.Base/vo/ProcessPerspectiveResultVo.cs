using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程透视搜索结果VO
/// </summary>
public class ProcessPerspectiveResultVo
{
    [JsonPropertyName("results")]
    public List<FormCodeResult> Results { get; set; } = new();

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("processedCount")]
    public int ProcessedCount { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    public class FormCodeResult
    {
        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("flowType")]
        public string FlowType { get; set; }

        [JsonPropertyName("latestMatch")]
        public VersionMatch LatestMatch { get; set; }

        [JsonPropertyName("allMatches")]
        public List<VersionMatch> AllMatches { get; set; } = new();
    }

    public class VersionMatch
    {
        [JsonPropertyName("confId")]
        public long ConfId { get; set; }

        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        [JsonPropertyName("effectiveStatus")]
        public int EffectiveStatus { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }
    }
}
