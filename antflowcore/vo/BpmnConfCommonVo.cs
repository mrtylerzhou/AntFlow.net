using antflowcore.vo;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BpmnConfCommonVo
    {
        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        [JsonPropertyName("processNum")]
        public string ProcessNum { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processDesc")]
        public string ProcessDesc { get; set; }

        [JsonPropertyName("viewPageButtons")]
        public BpmnConfViewPageButtonVo ViewPageButtons { get; set; }

        [JsonPropertyName("elementList")]
        public List<BpmnConfCommonElementVo> ElementList { get; set; }

        [JsonPropertyName("templateVos")]
        public List<BpmnTemplateVo> TemplateVos { get; set; }
    }
}