using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AntFlowCore.Core.vo;

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