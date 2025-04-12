using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.conf.json;
using antflowcore.vo;
using YourNamespace;

namespace AntFlowCore.Vo
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

        // Third party process
        [JsonPropertyName("formData")]
        public string FormData { get; set; }

        [JsonPropertyName("bpmnConfVo")]
        public BpmnConfVo BpmnConfVo { get; set; }

        [JsonPropertyName("accountType")]
        public int? AccountType { get; set; }

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
        public Dictionary<String,Object> LfConditions;
    }
}
