using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程千里眼 - 响应VO
/// </summary>
public class FlowClairvoyanceResultVo
{
    /// <summary>
    /// 命中的流程列表
    /// </summary>
    [JsonPropertyName("results")]
    public List<ProcessMatchResult> Results { get; set; } = new();

    /// <summary>
    /// 是否还有更多数据可扫描
    /// </summary>
    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    /// <summary>
    /// 下一次扫描的偏移量
    /// </summary>
    [JsonPropertyName("nextOffset")]
    public int NextOffset { get; set; }

    /// <summary>
    /// 本次扫描的流程数量
    /// </summary>
    [JsonPropertyName("scannedCount")]
    public int ScannedCount { get; set; }

    public class ProcessMatchResult
    {
        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        [JsonPropertyName("processKey")]
        public string ProcessKey { get; set; }

        [JsonPropertyName("processTypeName")]
        public string ProcessTypeName { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("processState")]
        public int? ProcessState { get; set; }

        [JsonPropertyName("matchedNodeCount")]
        public int MatchedNodeCount { get; set; }

        [JsonPropertyName("matchedPersonCount")]
        public int MatchedPersonCount { get; set; }

        [JsonPropertyName("matchedNodes")]
        public List<MatchedNode> MatchedNodes { get; set; } = new();
    }

    public class MatchedNode
    {
        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }

        [JsonPropertyName("elementName")]
        public string ElementName { get; set; }

        [JsonPropertyName("matchedPersons")]
        public List<MatchedPerson> MatchedPersons { get; set; } = new();
    }

    public class MatchedPerson
    {
        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }

        [JsonPropertyName("assigneeName")]
        public string AssigneeName { get; set; }
    }
}
