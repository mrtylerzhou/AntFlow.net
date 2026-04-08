using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class OutSidelevelAssignees
    {
        [JsonPropertyName("assigneeIds")]
        public List<string> AssigneeIds { get; set; }

        [JsonPropertyName("assigneeNameMap")]
        public Dictionary<int, string> AssigneeNameMap { get; set; }
    }
}