using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.vo;

namespace AntFlowCore.Vo
{
    public class BpmnStartConditionsVo
    {
        /// <summary>
        /// Gets or sets the process number.
        /// </summary>
        [JsonPropertyName("processNum")]
        public string ProcessNum { get; set; }

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the process description.
        /// </summary>
        [JsonPropertyName("processDesc")]
        public string ProcessDesc { get; set; }

        /// <summary>
        /// Gets or sets the business ID of the process.
        /// </summary>
        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        // Condition Fields

        /// <summary>
        /// Gets or sets the start user's ID.
        /// </summary>
        [JsonPropertyName("startUserId")]
        public string StartUserId { get; set; }

        /// <summary>
        /// Gets or sets the start user's name.
        /// </summary>
        [JsonPropertyName("startUserName")]
        public string StartUserName { get; set; }

        /// <summary>
        /// Gets or sets the start user's job level ID.
        /// </summary>
        [JsonPropertyName("startUserJobLevelId")]
        public long? StartUserJobLevelId { get; set; }

        /// <summary>
        /// Gets or sets the start user's department ID.
        /// </summary>
        [JsonPropertyName("startUserDeptId")]
        public long? StartUserDeptId { get; set; }

        /// <summary>
        /// Gets or sets the start user's services company ID.
        /// </summary>
        [JsonPropertyName("startUserServicesCompanyId")]
        public long? StartUserServicesCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the approval employee ID.
        /// </summary>
        [JsonPropertyName("approvalEmplId")]
        public string ApprovalEmplId { get; set; }

        /// <summary>
        /// Gets or sets the list of employee IDs.
        /// </summary>
        [JsonPropertyName("employeeIds")]
        public List<string> EmployeeIds { get; set; }

        /// <summary>
        /// Gets or sets the list of approvers.
        /// </summary>
        [JsonPropertyName("approversList")]
        public Dictionary<String,List<BaseIdTranStruVo>> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the entry ID.
        /// </summary>
        [JsonPropertyName("entryId")]
        public string EntryId { get; set; }

        /// <summary>
        /// Gets or sets the total money.
        /// </summary>
        [JsonPropertyName("totalMoney")]
        public string TotalMoney { get; set; }

        /// <summary>
        /// Gets or sets the out total money.
        /// </summary>
        [JsonPropertyName("outTotalMoney")]
        public string OutTotalMoney { get; set; }

        /// <summary>
        /// Gets or sets the total money operator.
        /// </summary>
        [JsonPropertyName("totalMoneyOperator")]
        public int? TotalMoneyOperator { get; set; }

        /// <summary>
        /// Gets or sets the list of multiple approval IDs.
        /// </summary>
        [JsonPropertyName("emplIdList")]
        public List<string> EmplIdList { get; set; }

        // Demo Fields

        /// <summary>
        /// Gets or sets the third-party account type.
        /// </summary>
        [JsonPropertyName("accountType")]
        public int? AccountType { get; set; }

        /// <summary>
        /// Gets or sets the job level object.
        /// </summary>
        [JsonPropertyName("jobLevelVo")]
        public BaseIdTranStruVo JobLevelVo { get; set; }

        /// <summary>
        /// Gets or sets the leave hour for the leave form.
        /// </summary>
        [JsonPropertyName("leaveHour")]
        public double? LeaveHour { get; set; }

        /// <summary>
        /// Gets or sets the purchase type for the purchase business form.
        /// </summary>
        [JsonPropertyName("purchaseType")]
        public int? PurchaseType { get; set; }

        /// <summary>
        /// Gets or sets the total procurement money for the plan.
        /// </summary>
        [JsonPropertyName("planProcurementTotalMoney")]
        public double? PlanProcurementTotalMoney { get; set; }

        /// <summary>
        /// Gets or sets the list of forwarded employees.
        /// </summary>
        [JsonPropertyName("empToForwardList")]
        public List<string> EmpToForwardList { get; set; } = new List<string>();

        // Third Party Process Fields

        /// <summary>
        /// Gets or sets whether it is a third-party process (0 for no, 1 for yes).
        /// </summary>
        [JsonPropertyName("isOutSideProcess")]
        public int? IsOutSideProcess { get; set; }

        /// <summary>
        /// Gets or sets the business party ID.
        /// </summary>
        [JsonPropertyName("businessPartyId")]
        public int? BusinessPartyId { get; set; }

        /// <summary>
        /// Gets or sets the template mark ID.
        /// </summary>
        [JsonPropertyName("templateMarkId")]
        public int? TemplateMarkId { get; set; }

        /// <summary>
        /// Gets or sets the outside type (1 for embedded, 2 for API access).
        /// </summary>
        [JsonPropertyName("outSideType")]
        public int? OutSideType { get; set; }

        /// <summary>
        /// Gets or sets whether it is an outside access process (API access process).
        /// </summary>
        [JsonPropertyName("isOutSideAccessProc")]
        public bool IsOutSideAccessProc { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of embedded nodes (for both embedded and API access processes).
        /// </summary>
        [JsonPropertyName("embedNodes")]
        public List<OutSideBpmAccessEmbedNodeVo> EmbedNodes { get; set; }

        /// <summary>
        /// Gets or sets the list of third-party level nodes.
        /// </summary>
        [JsonPropertyName("outSideLevelNodes")]
        public List<OutSideLevelNodeVo> OutSideLevelNodes { get; set; }

        /// <summary>
        /// Gets or sets the list of LF conditions.
        /// </summary>
        [JsonPropertyName("lfConditions")]
        public IDictionary<string, object> LfConditions { get; set; }
        public bool IsLowCodeFlow { get; set; }
        [JsonPropertyName("bpmnCode")]
        public String BpmnCode { get; set; }
        [JsonPropertyName("templateMarkIds")]
        public List<string> TemplateMarkIds { get; set; }
        [JsonPropertyName("isPreview")]
        public bool IsPreview { get; set; }
    }
}
