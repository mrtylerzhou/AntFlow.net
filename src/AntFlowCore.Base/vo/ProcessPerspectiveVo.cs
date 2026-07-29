using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程透视搜索请求VO
/// </summary>
public class ProcessPerspectiveVo
{
    [JsonPropertyName("formCodes")]
    public List<string> FormCodes { get; set; }

    [JsonPropertyName("versionMode")]
    public string VersionMode { get; set; }

    [JsonPropertyName("recentN")]
    public int? RecentN { get; set; }

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("batchSize")]
    public int? BatchSize { get; set; }

    [JsonPropertyName("filters")]
    public Filters FilterConfig { get; set; }

    public class Filters
    {
        [JsonPropertyName("bpmnNameLike")]
        public string? BpmnNameLike { get; set; }

        [JsonPropertyName("useExternalForm")]
        public bool? UseExternalForm { get; set; }

        [JsonPropertyName("formFieldKeyword")]
        public string? FormFieldKeyword { get; set; }

        [JsonPropertyName("hasEditableFieldPerm")]
        public bool? HasEditableFieldPerm { get; set; }

        [JsonPropertyName("approverRules")]
        public List<int>? ApproverRules { get; set; }

        [JsonPropertyName("hasAdditionalSign")]
        public bool? HasAdditionalSign { get; set; }

        [JsonPropertyName("hasExcludeSign")]
        public bool? HasExcludeSign { get; set; }

        [JsonPropertyName("noHeaderActions")]
        public List<int>? NoHeaderActions { get; set; }

        [JsonPropertyName("buttonTypes")]
        public List<int>? ButtonTypes { get; set; }

        [JsonPropertyName("hasNotice")]
        public bool? HasNotice { get; set; }

        [JsonPropertyName("nodeTypes")]
        public List<int>? NodeTypes { get; set; }

        [JsonPropertyName("deduplication")]
        public bool? Deduplication { get; set; }

        [JsonPropertyName("allowRevoke")]
        public bool? AllowRevoke { get; set; }

        [JsonPropertyName("allowCancel")]
        public bool? AllowCancel { get; set; }

        [JsonPropertyName("allowForward")]
        public bool? AllowForward { get; set; }
    }
}
