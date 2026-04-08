using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class BpmnNodeButtonConfBaseVo
    {
        [JsonPropertyName("startPage")]
        public List<int> StartPage { get; set; }

        [JsonPropertyName("approvalPage")]
        public List<int> ApprovalPage { get; set; }

        [JsonPropertyName("viewPage")]
        public List<int> ViewPage { get; set; }
    }
}