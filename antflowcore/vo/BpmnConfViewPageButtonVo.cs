using antflowcore.vo;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BpmnConfViewPageButtonVo
    {
        [JsonPropertyName("viewPageStart")]
        public List<BpmnConfCommonButtonPropertyVo> ViewPageStart { get; set; }

        [JsonPropertyName("viewPageOther")]
        public List<BpmnConfCommonButtonPropertyVo> ViewPageOther { get; set; }
    }
}