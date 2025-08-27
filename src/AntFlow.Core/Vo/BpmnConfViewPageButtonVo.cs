using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnConfViewPageButtonVo
{
    [JsonPropertyName("viewPageStart")] public List<BpmnConfCommonButtonPropertyVo> ViewPageStart { get; set; }

    [JsonPropertyName("viewPageOther")] public List<BpmnConfCommonButtonPropertyVo> ViewPageOther { get; set; }
}