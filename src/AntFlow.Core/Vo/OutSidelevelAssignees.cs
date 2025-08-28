using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSidelevelAssignees
{
    [JsonPropertyName("assigneeIds")] public List<string> AssigneeIds { get; set; }

    [JsonPropertyName("assigneeNameMap")] public Dictionary<int, string> AssigneeNameMap { get; set; }
}