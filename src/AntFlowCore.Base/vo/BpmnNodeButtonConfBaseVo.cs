using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class BpmnNodeButtonConfBaseVo
    {
        [JsonPropertyName("startPage")]
        public List<BpmnConfCommonButtonPropertyVo> StartPage { get; set; } = new List<BpmnConfCommonButtonPropertyVo>();

        [JsonPropertyName("approvalPage")]
        public List<BpmnConfCommonButtonPropertyVo> ApprovalPage { get; set; } = new List<BpmnConfCommonButtonPropertyVo>();

        [JsonPropertyName("viewPage")]
        public List<BpmnConfCommonButtonPropertyVo> ViewPage { get; set; } = new List<BpmnConfCommonButtonPropertyVo>();
    }
}
