using System.Text.Json.Serialization;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Base.vo
{
    public class ProcessRecordInfoVo
    {
        [JsonPropertyName("verifyInfoList")]
        public List<BpmVerifyInfoVo> VerifyInfoList { get; set; }

        [JsonPropertyName("employee")]
        public DetailedUser Employee { get; set; }

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

        /// <summary>
        /// 当前任务 FormKey, 存放 NodeExtraInfoDTO 序列化 JSON (包含节点标签).
        /// ProcessApprovalService.GetBusinessInfo 据此判断当前节点是否需要渲染
        /// [指定下一节点审批人] 按钮 (af_syslabel_appoint_next_node_approver 标签).
        /// 对应 Java ProcessRecordInfoVo.formKey.
        /// </summary>
        [JsonPropertyName("formKey")]
        public string FormKey { get; set; }

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

        public List<String> ViewNodeIds { get; set; }
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
