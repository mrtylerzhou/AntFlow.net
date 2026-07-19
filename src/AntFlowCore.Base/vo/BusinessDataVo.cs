using System.Text.Json.Serialization;
using AntFlowCore.Base.conf.json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.vo
{
    public class BusinessDataVo
    {
        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        [JsonPropertyName("processKey")]
        public string ProcessKey { get; set; }

        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        [JsonPropertyName("params")]
        public string Params { get; set; }

        [JsonPropertyName("processTitle")]
        public string ProcessTitle { get; set; }

        [JsonPropertyName("approvalComment")]
        public string ApprovalComment { get; set; }

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }

        [JsonPropertyName("processRecordInfo")]
        public ProcessRecordInfoVo ProcessRecordInfo { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("processState")]
        public bool? ProcessState { get; set; }

        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }
        [JsonPropertyName("taskDefKey")]
        public String TaskDefKey { get; set; }
        [JsonPropertyName("nodeId"),JsonConverter(typeof(IntToStringConverter))]
        public String NodeId { get; set; }
        [JsonPropertyName("elementId")]
        public String ElementId { get; set; }
        [JsonPropertyName("objectMap")]
        public Dictionary<string, object> ObjectMap { get; set; }

        [JsonPropertyName("moreHandlers")]
        public List<string> MoreHandlers { get; set; }

        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        [JsonPropertyName("operationType")]
        public int? OperationType { get; set; }

        [JsonPropertyName("userIds")] 
        public List<string> UserIds { get; set; } = new List<string>();

        [JsonPropertyName("userInfos")]
        public List<BaseIdTranStruVo> UserInfos { get; set; }

        [JsonPropertyName("approversList")] 
        public Dictionary<String,List<BaseIdTranStruVo>> ApproversList { get; set; } = new Dictionary<string, List<BaseIdTranStruVo>>();

        [JsonPropertyName("flag")]
        public bool? Flag { get; set; }

        [JsonPropertyName("initDatas")]
        public object InitDatas { get; set; }

        [JsonPropertyName("startUserId")]
        public string StartUserId { get; set; } = string.Empty;

        [JsonPropertyName("startUserName")]
        public string StartUserName { get; set; }

        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        [JsonPropertyName("emplId")]
        public string EmplId { get; set; }

        [JsonPropertyName("paramStr")]
        public string ParamStr { get; set; }

        [JsonPropertyName("empId")]
        public string EmpId { get; set; }

        [JsonPropertyName("processDigest")]
        public string ProcessDigest { get; set; }

        [JsonPropertyName("dataSourceId")]
        public long? DataSourceId { get; set; }

        [JsonPropertyName("empIds")]
        public List<string> EmpIds { get; set; }

        [JsonPropertyName("isSignUpNode")]
        public bool? IsSignUpNode { get; set; }

        [JsonPropertyName("signUpUsers")]
        public List<BaseIdTranStruVo> SignUpUsers { get; set; }=new List<BaseIdTranStruVo>();

        [JsonPropertyName("isStartPagePreview")]
        public bool? IsStartPagePreview { get; set; }

        [JsonPropertyName("backToEmployeeId")]
        public string BackToEmployeeId { get; set; }
        [JsonPropertyName("backToEmployeeName")]
        public string BackToEmployeeName { get; set; }
        [JsonPropertyName("backToModifyType")]
        public int? BackToModifyType { get; set; }
        [JsonPropertyName("backToNodeId"),JsonConverter(typeof(IntToStringConverter))]
        public String BackToNodeId { get; set; }
        // Third party process
        [JsonPropertyName("formData")]
        public string FormData { get; set; }

        [JsonPropertyName("bpmnConfVo")]
        public BpmnConfVo BpmnConfVo { get; set; }
        

        [JsonPropertyName("jobLevelVo")]
        public BaseIdTranStruVo JobLevelVo { get; set; }

        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }

        [JsonPropertyName("isOutSideAccessProc"),JsonConverter(typeof(StringToNullableBoolConverter))]
        public bool? IsOutSideAccessProc { get; set; } = false;

        [JsonPropertyName("isOutSideChecked")]
        public bool IsOutSideChecked { get; set; } = false;

        [JsonPropertyName("isLowCodeFlow"),JsonConverter(typeof(BooleanToNullableIntJsonConverter))]
        public int? IsLowCodeFlow { get; set; } = 0;

        /// <summary>
        /// Whether this submit is a migration (dynamic condition re-evaluation) of a running
        /// process. When true the original process number is reused and no new BpmBusinessProcess
        /// is created; the existing one is updated with the new instance id.
        /// </summary>
        [JsonPropertyName("isMigration")]
        public bool? IsMigration { get; set; }

        [JsonPropertyName("bpmFlowCallbackUrl")]
        public string BpmFlowCallbackUrl { get; set; }

        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; }

        [JsonPropertyName("submitUrl")]
        public string SubmitUrl { get; set; }

        [JsonPropertyName("submitUser")]
        public string SubmitUser { get; set; }

        [JsonPropertyName("conditionsUrl")]
        public string ConditionsUrl { get; set; }

        [JsonPropertyName("outSideType")]
        public int? OutSideType { get; set; }

        [JsonPropertyName("templateMark")]
        public string TemplateMark { get; set; }

        [JsonPropertyName("templateMarkId")]
        public int? TemplateMarkId { get; set; }

        [JsonPropertyName("embedNodes")]
        public List<OutSideBpmAccessEmbedNodeVo> EmbedNodes { get; set; }

        [JsonPropertyName("outSideLevelNodes")]
        public List<OutSideLevelNodeVo> OutSideLevelNodes { get; set; }

        [JsonPropertyName("msgProcessEventEnum")]
        public MsgProcessEventEnum MsgProcessEventEnum { get; set; }

        [JsonPropertyName("templateMarkIds")]
        public List<string> TemplateMarkIds { get; set; }

        [JsonPropertyName("lfConditions")]
        public Dictionary<String,Object> LfConditions { get; set; }
        
        [JsonPropertyName("approvalEmpls")]
        public List<BaseIdTranStruVo> ApprovalEmpls { get; set; }

        /// <summary>
        /// 上一节点审批人通过[指定下一节点审批人]按钮选择的下一节点实际审批人.
        /// 由 ButtonOperationService 写入 ThreadLocalContainer, AFTaskService.InsertTasks 读取并替换虚拟审批人 -4.
        /// 简化规则: 仅允许 1 人.
        /// </summary>
        [JsonPropertyName("nextNodeApprovers")]
        public List<BaseIdTranStruVo> NextNodeApprovers { get; set; }

        /// <summary>
        /// 低代码表单字段及值. 由 UDLFApplyVo 上移至 BusinessDataVo,
        /// 以便 NextNodeLabelsProcessor 等运行时处理器无需类型转换即可访问.
        /// 对应 Java BusinessDataVo.lfFields.
        /// </summary>
        [JsonPropertyName("lfFields")]
        public Dictionary<string, object> LfFields { get; set; }

        /// <summary>
        /// key is node id, value is a list of form-related assignee ids extracted from form data.
        /// Populated by LowFlowApprovalService.ProcessFormRelatedUserConf during process start.
        /// </summary>
        [JsonPropertyName("node2formRelatedAssignees")]
        public Dictionary<string, List<string>> Node2formRelatedAssignees { get; set; }
    }
}
