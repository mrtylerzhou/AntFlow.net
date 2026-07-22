using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Core.vo;
using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

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
        [JsonPropertyName("isParallel")]
        public bool? IsParallel { get; set; }
        [JsonPropertyName("isDynamicCondition")]
        public bool? IsDynamicCondition { get; set; }
        [JsonPropertyName("aggregationNode")]
        public bool? AggregationNode { get; set; }
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
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

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

        [JsonPropertyName("extraFlags")]
        public int? ExtraFlags { get; set; }

        [JsonPropertyName("nodeTo")]
        public List<string> NodeTo { get; set; }

        [JsonPropertyName("property")]
        public BpmnNodePropertysVo Property { get; set; }

        [JsonPropertyName("params")]
        public BpmnNodeParamsVo Params { get; set; }

        [JsonPropertyName("buttons")]
        public BpmnNodeButtonConfBaseVo Buttons { get; set; }=new BpmnNodeButtonConfBaseVo();

        [JsonPropertyName("templateVos")]
        public List<BpmnTemplateVo> TemplateVos { get; set; }

        [JsonPropertyName("approveRemindVo")]
        public BpmnApproveRemindVo ApproveRemindVo { get; set; }

        [JsonPropertyName("labelList")]
        public List<BpmnNodeLabelVO> LabelList { get; set; }

        [JsonPropertyName("isCarbonCopyNode")]
        public bool? IsCarbonCopyNode { get; set; }

        /// <summary>
        /// Whether this node is a condition-approve node (nodeType=12 at design time,
        /// converted to nodeType=4 at runtime by AfNodeUtils.NodeSpecialProcess).
        /// Condition-approve node auto-completes only when condition is true;
        /// otherwise waits for human approval.
        /// </summary>
        [JsonPropertyName("isConditionApproveNode")]
        public bool? IsConditionApproveNode { get; set; }

        /// <summary>
        /// Whether this node is a condition-copy node (nodeType=13 at design time,
        /// converted to nodeType=4 at runtime by AfNodeUtils.NodeSpecialProcess).
        /// Condition-copy node always completes; only writes BpmProcessForward
        /// when condition is true.
        /// </summary>
        [JsonPropertyName("isConditionCopyNode")]
        public bool? IsConditionCopyNode { get; set; }

        /// <summary>
        /// Whether this node is an automatic node (nodeType=9 at design time,
        /// converted to nodeType=4 at runtime by AfNodeUtils.NodeSpecialProcess).
        /// Automatic node always completes after evaluating conditions and executing actions.
        /// </summary>
        [JsonPropertyName("isAutomaticNode")]
        public bool? IsAutomaticNode { get; set; }

        /// <summary>
        /// Auto-node style condition configuration. Used by condition-approve (12)
        /// and condition-copy (13) nodes to store conditionList + groupRelation.
        /// Front-end submits under JSON key "autoNodeConf" (shared with Java version).
        /// Persisted to node_config_json.autoNodeConf at design time and read back
        /// for runtime condition evaluation.
        /// </summary>
        [JsonPropertyName("autoNodeConf")]
        public AutoNodeConfVo? AutoNodeConf { get; set; }

        /// <summary>
        /// 标识当前节点为"上一节点指定"审批人类型.
        /// 前端传入,后端在 AfNodeUtils.NodeSpecialProcess 中据此自动贴 af_syslabel_prev_node_appointed 标签.
        /// 对应 Java BpmnNodeVo.isPrevNodeAppointed.
        /// </summary>
        [JsonPropertyName("isPrevNodeAppointed")]
        public bool? IsPrevNodeAppointed { get; set; }

        /// <summary>
        /// 添加标签到 LabelList, 若 LabelList 为空则初始化.
        /// 对应 Java BpmnNodeVo.setOrAddLabelList.
        /// </summary>
        public void SetOrAddLabelList(BpmnNodeLabelVO labelVO)
        {
            if (LabelList != null && LabelList.Count > 0)
            {
                LabelList.Add(labelVO);
            }
            else
            {
                LabelList = new List<BpmnNodeLabelVO> { labelVO };
            }
        }

        [JsonPropertyName("overtimeConf")]
        public TemplateOvertimeConf OvertimeConf { get; set; }

        [JsonPropertyName("operationTypes")]
        public List<int> OperationTypes { get; set; }

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
        [JsonPropertyName("noHeaderAction")]
        public int? NoHeaderAction { get; set; }
        private string? _nodeConfigJson;
        [JsonIgnore]
        public string? NodeConfigJson
        {
            get => _nodeConfigJson;
            set
            {
                _nodeConfigJson = value;
                if (!string.IsNullOrEmpty(value))
                {
                    _nodeConfigJsonObj = JsonConfUtil.ParseNodeConfig(value);
                }
            }
        }

        [JsonIgnore]
        private BpmnNodeConfigJson? _nodeConfigJsonObj;

        [JsonIgnore]
        public BpmnNodeConfigJson? NodeConfigJsonObj
        {
            get
            {
                if (_nodeConfigJsonObj == null && !string.IsNullOrEmpty(NodeConfigJson))
                {
                    _nodeConfigJsonObj = JsonConfUtil.ParseNodeConfig(NodeConfigJson);
                }
                return _nodeConfigJsonObj;
            }
            set => _nodeConfigJsonObj = value;
        }

        public BpmnNodeConfigJson GetOrCreateNodeConfigJson()
        {
            _nodeConfigJsonObj ??= new BpmnNodeConfigJson();
            return _nodeConfigJsonObj;
        }

        public string? SerializeNodeConfigJson()
        {
            if (_nodeConfigJsonObj == null)
            {
                return null;
            }
            return JsonConfUtil.ToNodeConfigJson(_nodeConfigJsonObj);
        }
    }

    /// <summary>
    /// Condition configuration for condition-approve / condition-copy nodes.
    /// Mirrors Java AutoNodeConf: { conditionList, groupRelation }.
    /// conditionList is the same nested-array structure used by condition nodes
    /// (nodeType=3) and is converted to BpmnNodeConditionsConfBaseVo at runtime
    /// via BpmnConfNodePropertyConverter.FromVue3Model.
    /// </summary>
    public class AutoNodeConfVo
    {
        [JsonPropertyName("conditionList")]
        public List<List<BpmnNodeConditionsConfVueVo>>? ConditionList { get; set; }

        [JsonPropertyName("groupRelation")]
        public bool? GroupRelation { get; set; }
    }