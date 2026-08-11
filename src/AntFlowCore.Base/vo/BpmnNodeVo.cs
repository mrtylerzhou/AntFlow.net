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
        /// 条件推进节点标记:条件审批(nodeType=12)子类型,自动勾选推进按钮(42,别名同意),强制 forwardType=2.
        /// 满足条件时自动推进到固定目标(虚拟人),不满足时留给真实审批人(可手动推进).
        /// 前端提交该字段,AfNodeUtils.NodeSpecialProcess 据此贴 condition_advance_node 标签.
        /// </summary>
        [JsonPropertyName("isConditionAdvanceNode")]
        public bool? IsConditionAdvanceNode { get; set; }

        /// <summary>
        /// 条件完成节点标记:条件推进(nodeType=12)子类型,目标设计时自动算最后一个审批人节点(不可编辑).
        /// 运行时复用条件推进处理器.满足条件自动推进到最后一个审批人,不满足留给真实审批人.
        /// </summary>
        [JsonPropertyName("isConditionFinishNode")]
        public bool? IsConditionFinishNode { get; set; }

        /// <summary>
        /// 条件拒绝节点标记:条件审批(nodeType=12)子类型.
        /// 满足条件时自动拒绝(固定终止流程,忽略不同意退回配置),不满足时留给真实审批人.
        /// 前端提交该字段,BpmnConfBizService 据此贴 condition_disagree_node 标签.
        /// </summary>
        [JsonPropertyName("isConditionDisagreeNode")]
        public bool? IsConditionDisagreeNode { get; set; }

        /// <summary>
        /// 条件自动加批节点标记:条件审批(nodeType=12)子类型.
        /// 满足条件时自动加批(autoSignUpUsers),不满足时留给真实审批人(加批按钮屏蔽).
        /// </summary>
        [JsonPropertyName("isConditionAutoSignUpNode")]
        public bool? IsConditionAutoSignUpNode { get; set; }

        /// <summary>
        /// 条件自动加批节点的加批人列表(必填), 存 nodeConfigJson.autoSignUpUsers.
        /// </summary>
        [JsonPropertyName("autoSignUpUsers")]
        public List<BaseIdTranStruVo>? AutoSignUpUsers { get; set; }

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
        /// Whether this node is an assist node (nodeType=17 at design time,
        /// converted to nodeType=4 at runtime by AfNodeUtils.NodeSpecialProcess).
        /// Assist node means 'handle' not 'approve', delegates to agree logic for flow progression.
        /// </summary>
        [JsonPropertyName("isAssistNode")]
        public bool? IsAssistNode { get; set; }

        /// <summary>Whether this node is an auto-advance node (nodeType=18 at design time, converted to nodeType=4 at runtime by AfNodeUtils.NodeSpecialProcess). Auto-advance node advances to a specified target node when condition is true, otherwise auto-completes like automatic node.</summary>
        [JsonPropertyName("isAutoAdvanceNode")]
        public bool? IsAutoAdvanceNode { get; set; }

        /// <summary>Whether this node is an auto-return node (nodeType=19 at design time, converted to nodeType=4 at runtime). Auto-return node returns to a specified target node when condition is true (FOUR_DISAGREE), otherwise auto-completes like automatic node.</summary>
        [JsonPropertyName("isAutoReturnNode")]
        public bool? IsAutoReturnNode { get; set; }

        /// <summary>条件退回节点标记:设计期 nodeType=20,运行期转为4,保留真实审批人.</summary>
        [JsonPropertyName("isConditionReturnNode")]
        public bool? IsConditionReturnNode { get; set; }
        /// <summary>条件退回发起人节点标记:设计期 nodeType=21,运行期转为4,保留真实审批人.</summary>
        [JsonPropertyName("isConditionReturnStarterNode")]
        public bool? IsConditionReturnStarterNode { get; set; }

        /// <summary>完成审批节点标记:审批人节点(nodeType=4)+推进按钮(42)+finish_approve_node标签,目标自动填充为流程最后一个审批人节点.前端提交该字段,AfNodeUtils.NodeSpecialProcess 据此贴标签.</summary>
        [JsonPropertyName("isFinishApproveNode")]
        public bool? IsFinishApproveNode { get; set; }

        /// <summary>
        /// 同意推进节点标记:普通审批人节点(nodeType=4)勾选同意按钮"跳转至固定节点"选项,强制 forwardType=2.
        /// 运行时通过同意按钮触发推进逻辑,与推进按钮(42)互斥.
        /// 前端提交该字段,AfNodeUtils.NodeSpecialProcess 据此贴 approve_forward_node 标签.
        /// </summary>
        [JsonPropertyName("isApproveForwardNode")]
        public bool? IsApproveForwardNode { get; set; }

        /// <summary>
        /// 自动完成节点标记:自动推进(nodeType=18)子类型,目标自动为最后一个审批人节点,不可编辑.
        /// 仅前端反显区分+颜色区分用,运行时复用 auto_advance_node 处理器.
        /// 前端提交该字段,AfNodeUtils.NodeSpecialProcess 据此贴 auto_complete_node 标签.
        /// </summary>
        [JsonPropertyName("isAutoCompleteNode")]
        public bool? IsAutoCompleteNode { get; set; }

        /// <summary>
        /// 推进行为类型: 2=固定节点(指定目标节点,运行时自动推进). 对应 Java BpmnNodeVo.forwardType.
        /// 前端提交,保存时持久化到 node_config_json.forwardType,反显时读回.
        /// </summary>
        [JsonPropertyName("forwardType")]
        public int? ForwardType { get; set; }

        /// <summary>
        /// 推进目标节点UUID列表(仅固定节点模式有效). 对应 Java BpmnNodeVo.forwardNodeIds.
        /// 前端提交,保存时持久化到 node_config_json.forwardNodeIds,反显时读回.
        /// </summary>
        [JsonPropertyName("forwardNodeIds")]
        public List<string>? ForwardNodeIds { get; set; }

        /// <summary>
        /// Auto-node style condition configuration. Used by condition-approve (12)
        /// and condition-copy (13) nodes to store conditionList + groupRelation.
        /// Front-end submits under JSON key "autoNodeConf" (shared with Java version).
        /// Persisted to node_config_json.autoNodeConf at design time and read back
        /// for runtime condition evaluation.
        /// </summary>
        [JsonPropertyName("autoNodeConf")]
        public BpmnNodeAutoNodeConfJson? AutoNodeConf { get; set; }

        /// <summary>
        /// 标识当前节点为"上一节点指定"审批人类型.
        /// 前端传入,后端在 AfNodeUtils.NodeSpecialProcess 中据此自动贴 af_syslabel_prev_node_appointed 标签.
        /// 对应 Java BpmnNodeVo.isPrevNodeAppointed.
        /// </summary>
        [JsonPropertyName("isPrevNodeAppointed")]
        public bool? IsPrevNodeAppointed { get; set; }

        /// <summary>
        /// 标识当前节点为"选择条件"审批人类型.
        /// 前端传入,后端在 BpmnConfBizService.Edit 中据此验证子节点包含动态条件网关并贴标签.
        /// 对应 Java BpmnNodeVo.isPickCondition.
        /// </summary>
        [JsonPropertyName("isPickCondition")]
        public bool? IsPickCondition { get; set; }

        /// <summary>
        /// 退回按钮行为类型(0=无限制, 1=上一节点, 2=发起人(不回), 3=发起人(回), 4=指定节点(不回), 5=指定节点(回))
        /// 前端传入,后端在 AfNodeUtils.NodeSpecialProcess 中据此自动贴退回行为标签.
        /// </summary>
        [JsonPropertyName("drawBackType")]
        public int? DrawBackType { get; set; }

        /// <summary>
        /// 退回按钮允许退回的节点ID列表(设计态nodeId UUID). 仅 drawBackType=4/5 时有值.
        /// </summary>
        [JsonPropertyName("drawBackNodeIds")]
        public List<string>? DrawBackNodeIds { get; set; }

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
