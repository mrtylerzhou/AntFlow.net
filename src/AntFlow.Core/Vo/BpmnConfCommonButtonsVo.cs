using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnConfCommonButtonsVo
{
    [JsonPropertyName("startPage")] public List<BpmnConfCommonButtonPropertyVo> StartPage { get; set; }

    [JsonPropertyName("approvalPage")] public List<BpmnConfCommonButtonPropertyVo> ApprovalPage { get; set; }

    [JsonPropertyName("viewPage")] public List<BpmnConfCommonButtonPropertyVo> ViewPage { get; set; }
}