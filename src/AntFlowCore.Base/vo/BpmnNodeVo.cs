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
        //===============>>ext fields<<===================

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

        /// <summary>
        /// Node labels (user-defined or dynamically added). Persisted into
        /// node_config_json.buttonSignConf.labels at design time and read back
        /// for runtime use. Eventually carried on BpmnConfCommonElementVo.LabelList.
        /// </summary>
        [JsonPropertyName("labelList")]
        public List<BpmnNodeLabelVO> LabelList { get; set; }

        /// <summary>
        /// Whether this node is a carbon-copy (抄送) node V2 (enters the engine).
        /// Derived from LabelList at read time.
        /// </summary>
        [JsonIgnore]
        public bool? IsCarbonCopyNode { get; set; }

        /// <summary>
        /// Overtime notice configuration (migrated from bpm_process_node_overtime).
        /// Written to node_config_json.templateConf.overtimeConf during edit.
        /// </summary>
        [JsonPropertyName("overtimeConf")]
        public TemplateOvertimeConf OvertimeConf { get; set; }

        /// <summary>
        /// Operation types for this node (migrated from bpm_process_operation).
        /// Written to node_config_json.buttonSignConf.operationTypes during edit.
        /// </summary>
        [JsonPropertyName("operationTypes")]
        public List<int> OperationTypes { get; set; }

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

        /// <summary>
        /// 外部表单模式: 节点级整表隐藏标记
        /// Key = formdataId, Value = true 表示该表单在此节点整体隐藏
        /// 仅外部表单模式使用; 内联模式为 null
        /// </summary>
        [JsonPropertyName("formHidden")]
        public Dictionary<string, bool> FormHidden { get; set; }

        [JsonPropertyName("fromNodes")]
        public List<BpmnNodeVo> FromNodes { get; set; }

        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }
        [JsonPropertyName("noHeaderAction")]
        public int? NoHeaderAction { get; set; }

        /// <summary>
        /// Whether this node is an automatic node (UX abstraction over approver node
        /// with automaticNode label and virtual assignee AUTO_NODE_SKIP(-3)).
        /// Derived from LabelList at read time.
        /// </summary>
        [JsonIgnore]
        public bool? IsAutomaticNode { get; set; }

        /// <summary>
        /// Auto node condition configuration (received from frontend during edit,
        /// sent to frontend during display). Stored in node_config_json.autoNodeConf.
        /// </summary>
        [JsonPropertyName("autoNodeConf")]
        public BpmnNodeAutoNodeConfJson? AutoNodeConf { get; set; }
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
