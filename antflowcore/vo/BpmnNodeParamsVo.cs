using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class BpmnNodeParamsVo
    {
        [JsonPropertyName("nodeTo")]
        public string NodeTo { get; set; }

        [JsonPropertyName("paramType")]
        public int ParamType { get; set; }

        [JsonPropertyName("assignee")]
        public BpmnNodeParamsAssigneeVo Assignee { get; set; }

        [JsonPropertyName("assigneeList")]
        public List<BpmnNodeParamsAssigneeVo> AssigneeList { get; set; }

        [JsonPropertyName("isNodeDeduplication")]
        public int IsNodeDeduplication { get; set; } = 0;
    }
}