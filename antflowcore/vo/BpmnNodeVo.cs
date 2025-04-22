using System.Text.Json.Serialization;

namespace antflowcore.vo;

   public class BpmnNodeVo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("confId")]
        public long ConfId { get; set; }

        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }

        [JsonPropertyName("nodeType")]
        public int NodeType { get; set; }

        [JsonPropertyName("nodeProperty")]
        public int? NodeProperty { get; set; }

        [JsonPropertyName("nodePropertyName")]
        public string NodePropertyName { get; set; }

        [JsonPropertyName("nodeFrom")]
        public string NodeFrom { get; set; }

        private string _nodeFroms;
        [JsonPropertyName("nodeFroms")]
        public string NodeFroms
        {
            get { return _nodeFroms; }
            set
            {
                _nodeFroms = value;
                if (!string.IsNullOrEmpty(value))
                {
                    // 当设置 NodeFroms 时，自动更新 PrevId
                    PrevId = new List<string>(value.Split(','));
                }
            }
        }

        [JsonPropertyName("prevId")]
        private List<string> _prevId = new List<string>();
        public List<string> PrevId
        {
            get { return _prevId; }
            set
            {
                _prevId = value;
                if (_prevId != null && _prevId.Count > 0)
                {
                    // 当设置 PrevId 时，自动更新 NodeFroms
                    NodeFroms = string.Join(",", _prevId);
                }
            }
        }

        [JsonPropertyName("batchStatus")]
        public int BatchStatus { get; set; }

        [JsonPropertyName("approvalStandard")]
        public int ApprovalStandard { get; set; }

        [JsonPropertyName("nodeName")]
        public string NodeName { get; set; }

        [JsonPropertyName("nodeDisplayName")]
        public string NodeDisplayName { get; set; }

        [JsonPropertyName("annotation")]
        public string Annotation { get; set; }

        [JsonPropertyName("isDeduplication")]
        public int IsDeduplication { get; set; }

        [JsonPropertyName("deduplicationExclude")]
        public bool DeduplicationExclude { get; set; }

        [JsonPropertyName("isSignUp")]
        public int IsSignUp { get; set; }

        [JsonPropertyName("orderedNodeType")]
        public int? OrderedNodeType { get; set; }

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

        //===============>>ext fields<<===================

        [JsonPropertyName("nodeTo")]
        public List<string> NodeTo { get; set; }

        [JsonPropertyName("property")]
        public BpmnNodePropertysVo Property { get; set; }

        [JsonPropertyName("params")]
        public BpmnNodeParamsVo Params { get; set; }

        [JsonPropertyName("buttons")]
        public BpmnNodeButtonConfBaseVo Buttons { get; set; }

        [JsonPropertyName("templateVos")]
        public List<BpmnTemplateVo> TemplateVos { get; set; }

        [JsonPropertyName("approveRemindVo")]
        public BpmnApproveRemindVo ApproveRemindVo { get; set; }

        //===============>>third party processs service<<===================

        [JsonPropertyName("conditionsUrl")]
        public string ConditionsUrl { get; set; }

        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        [JsonPropertyName("isOutSideProcess")]
        public int? IsOutSideProcess { get; set; }

        [JsonPropertyName("isLowCodeFlow")]
        public int? IsLowCodeFlow { get; set; }

        [JsonPropertyName("lfFieldControlVOs")]
        public List<LFFieldControlVO> LfFieldControlVOs { get; set; }

        [JsonPropertyName("fromNodes")]
        public List<BpmnNodeVo> FromNodes { get; set; }

        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }
    }