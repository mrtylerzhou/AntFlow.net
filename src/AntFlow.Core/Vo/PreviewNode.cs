using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class PreviewNode
{
    [JsonPropertyName("bpmnName")] public string BpmnName { get; set; }

    [JsonPropertyName("formCode")] public string FormCode { get; set; }

    [JsonPropertyName("bpmnNodeList")] public List<BpmnNodeVo> BpmnNodeList { get; set; }

    [JsonPropertyName("startUserInfo")] public PrevEmployeeInfo StartUserInfo { get; set; }

    [JsonPropertyName("employeeInfo")] public PrevEmployeeInfo EmployeeInfo { get; set; }

    [JsonPropertyName("deduplicationType")]
    public int? DeduplicationType { get; set; }

    [JsonPropertyName("deduplicationTypeName")]
    public string DeduplicationTypeName { get; set; }

    [JsonPropertyName("currentNodeId")] public string CurrentNodeId { get; set; }

    [JsonPropertyName("beforeNodeIds")] public List<string> BeforeNodeIds { get; set; }

    [JsonPropertyName("afterNodeIds")] public List<string> AfterNodeIds { get; set; }
}