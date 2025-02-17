using System.Text.Json.Serialization;

namespace antflowcore.vo;

  public class BpmnConfVo
    {
        /// <summary>
        /// auto incr Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// bpmnCode
        /// </summary>
        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// bpmnName
        /// </summary>
        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        /// <summary>
        /// process's type
        /// </summary>
        [JsonPropertyName("bpmnType")]
        public int BpmnType { get; set; }

        /// <summary>
        /// formCode
        /// </summary>
        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        [JsonPropertyName("appId")]
        public int? AppId { get; set; }

        [JsonPropertyName("deduplicationType")]
        public int? DeduplicationType { get; set; }

        [JsonPropertyName("effectiveStatus")]
        public int EffectiveStatus { get; set; }

        [JsonPropertyName("isAll")]
        public int IsAll { get; set; }

        [JsonPropertyName("isOutSideProcess")]
        public int? IsOutSideProcess { get; set; }

        [JsonPropertyName("isLowCodeFlow")]
        public int? IsLowCodeFlow { get; set; }

        /// <summary>
        /// process's business party
        /// </summary>
        [JsonPropertyName("businessPartyId")]
        public int? BusinessPartyId { get; set; }

        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        [JsonPropertyName("isDel")]
        public int IsDel { get; set; }

        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        //===============>>query to do list<<===================

        [JsonPropertyName("search")]
        public string Search { get; set; }

        //===============>>extend info<<===================

        [JsonPropertyName("deduplicationTypeName")]
        public string DeduplicationTypeName { get; set; }

        [JsonPropertyName("createUserName")]
        public string CreateUserName { get; set; }

        [JsonPropertyName("createUserUuid")]
        public string CreateUserUuid { get; set; }

        [JsonPropertyName("businessPartyName")]
        public string BusinessPartyName { get; set; }

        [JsonPropertyName("businessPartyMark")]
        public string BusinessPartyMark { get; set; }

        [JsonPropertyName("nodes")]
        public List<BpmnNodeVo> Nodes { get; set; }

        [JsonPropertyName("viewPageButtons")]
        public BpmnViewPageButtonBaseVo ViewPageButtons { get; set; }

        [JsonPropertyName("templateVos")]
        public List<BpmnTemplateVo> TemplateVos { get; set; }

        //===============>>thirdy party process<<===================

        [JsonPropertyName("formData")]
        public string FormData { get; set; }

        [JsonPropertyName("bpmConfCallbackUrl")]
        public string BpmConfCallbackUrl { get; set; }

        [JsonPropertyName("bpmFlowCallbackUrl")]
        public string BpmFlowCallbackUrl { get; set; }

        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; }

        [JsonPropertyName("submitUrl")]
        public string SubmitUrl { get; set; }

        [JsonPropertyName("conditionsUrl")]
        public string ConditionsUrl { get; set; }

        [JsonPropertyName("businessPartyIds")]
        public List<long> BusinessPartyIds { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("lfFormData")]
        public string LfFormData { get; set; }

        [JsonPropertyName("lfFormDataId")]
        public long? LfFormDataId { get; set; }
    }