using System;
using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class BpmnNodeParamsAssigneeVo
    {
        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }

        [JsonPropertyName("assigneeName")]
        public string AssigneeName { get; set; }

        [JsonPropertyName("elementName")]
        public string ElementName { get; set; }

        [JsonPropertyName("isDeduplication")]
        public int IsDeduplication { get; set; } = 0;
    }
}