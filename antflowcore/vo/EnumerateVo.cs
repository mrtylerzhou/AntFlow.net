using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.vo;

namespace AntFlowCore.Vo
{
    public class EnumerateVo
    {
        [JsonPropertyName("code")] 
        public int Code { get; set; }

        [JsonPropertyName("desc")] 
        public string Desc { get; set; }

        // 是否节点内属性
        [JsonPropertyName("isInNode")] 
        public bool IsInNode { get; set; }

        // 节点类型（1发起节点 4审批节点 0节点外）
        [JsonPropertyName("nodeType")]
        public int NodeType { get; set; }

        // 默认的消息模板
        [JsonPropertyName("vo")] 
        public BaseIdTranStruVo Vo { get; set; }

        // 默认的通知对象
        [JsonPropertyName("informIdList")] 
        public List<int> InformIdList { get; set; }

        [JsonPropertyName("processCode")] 
        public string ProcessCode { get; set; }
    }
}