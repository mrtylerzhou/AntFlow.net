using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnNodeParamsAssigneeVo
{
    [JsonPropertyName("assignee")] public string Assignee { get; set; }

    [JsonPropertyName("assigneeName")] public string AssigneeName { get; set; }

    [JsonPropertyName("elementName")] public string ElementName { get; set; }

    [JsonPropertyName("isDeduplication")] public int IsDeduplication { get; set; } = 0;
}