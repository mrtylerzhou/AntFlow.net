using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AntFlowCore.Entity;
using antflowcore.vo;

namespace AntFlowCore.Vo
{
    public class ProcessRecordInfoVo
    {
        [JsonPropertyName("verifyInfoList")]
        public List<BpmVerifyInfoVo> VerifyInfoList { get; set; }

        [JsonPropertyName("employee")]
        public Employee Employee { get; set; }

        [JsonPropertyName("processTitle")]
        public string ProcessTitle { get; set; }

        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        [JsonPropertyName("startUserId")]
        public string StartUserId { get; set; }

        [JsonPropertyName("nodeType")]
        public int? NodeType { get; set; }

        [JsonPropertyName("disagreeType")]
        public int? DisagreeType { get; set; }

        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        [JsonPropertyName("options")]
        public List<ProcessActionButtonVo> Options { get; set; }

        [JsonPropertyName("appBelowOptions")]
        public List<ProcessActionButtonVo> AppBelowOptions { get; set; }

        [JsonPropertyName("type")]
        public int? Type { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("taskState")]
        public string TaskState { get; set; }

        [JsonPropertyName("pcButtons")]
        public Dictionary<string, List<ProcessActionButtonVo>> PcButtons { get; set; }

        [JsonPropertyName("appButtons")]
        public Dictionary<string, Dictionary<string, List<ProcessActionButtonVo>>> AppButtons { get; set; }

        [JsonPropertyName("initiatePcButtons")]
        public List<ProcessActionButtonVo> InitiatePcButtons { get; set; }

        [JsonPropertyName("initiateAppButtons")]
        public Dictionary<string, List<ProcessActionButtonVo>> InitiateAppButtons { get; set; }

        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }

        [JsonPropertyName("isCustomNode")]
        public bool? IsCustomNode { get; set; }

        [JsonPropertyName("processCode")]
        public string ProcessCode { get; set; }

        [JsonPropertyName("processKey")]
        public string ProcessKey { get; set; }

        [JsonPropertyName("initDatas")]
        public object InitDatas { get; set; }

        [JsonPropertyName("lfFieldControlVOs")]
        public List<LFFieldControlVO> LfFieldControlVOs { get; set; }
    }
}
