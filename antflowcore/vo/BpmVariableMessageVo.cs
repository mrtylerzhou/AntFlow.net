using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AntFlowCore.Constants;
using AntFlowCore.Enums;
using AntFlowCore.Vo;

namespace AntFlowCore.Vo
{
    public class BpmVariableMessageVo
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the process variable ID.
        /// </summary>
        [JsonPropertyName("variableId")]
        public long? VariableId { get; set; }

        /// <summary>
        /// Gets or sets the process flow element ID.
        /// </summary>
        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }

        /// <summary>
        ///  1-out of node message；2-in node message
        /// </summary>
        [JsonPropertyName("messageType")]
        public int? MessageType { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonPropertyName("eventType")]
        public int? EventType { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the remark.
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

        /// <summary>
        /// Gets or sets whether the item is deleted (0 for valid, 1 for deleted).
        /// </summary>
        [JsonPropertyName("isDel")]
        public int? IsDel { get; set; }

        /// <summary>
        /// Gets or sets the creator's username.
        /// </summary>
        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the updater's username.
        /// </summary>
        [JsonPropertyName("updateUser")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Gets or sets the update time.
        /// </summary>
        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }

        // Extended Fields

        /// <summary>
        /// Gets or sets the process instance ID.
        /// </summary>
        [JsonPropertyName("processInsId")]
        public string ProcessInsId { get; set; }

        /// <summary>
        /// Gets or sets the BPMN code of the process.
        /// </summary>
        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// Gets or sets the BPMN name of the process.
        /// </summary>
        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        /// <summary>
        /// Gets or sets the process number (also referred to as process code in other contexts).
        /// </summary>
        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        /// <summary>
        /// Gets or sets the process type.
        /// </summary>
        [JsonPropertyName("processType")]
        public string ProcessType { get; set; }

        /// <summary>
        /// Gets or sets the process task ID (task specified by BPMN).
        /// </summary>
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        /// <summary>
        /// Gets or sets the form code.
        /// </summary>
        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        /// <summary>
        /// Gets or sets the operation type.
        /// </summary>
        [JsonPropertyName("type")]
        public int? Type { get; set; }

        /// <summary>
        /// Gets or sets the username of the applier.
        /// </summary>
        [JsonPropertyName("startUser")]
        public string StartUser { get; set; }

        /// <summary>
        /// Gets or sets the current assignee.
        /// </summary>
        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }

        /// <summary>
        /// Gets or sets the list of already approved users.
        /// </summary>
        [JsonPropertyName("approveds")]
        public List<string> Approveds { get; set; }

        /// <summary>
        /// Gets or sets the list of forwarded users.
        /// </summary>
        [JsonPropertyName("forwardUsers")]
        public List<string> ForwardUsers { get; set; }

        /// <summary>
        /// Gets or sets the list of node sign-up users.
        /// </summary>
        [JsonPropertyName("signUpUsers")]
        public List<string> SignUpUsers { get; set; }

        /// <summary>
        /// Gets or sets the approval employee ID.
        /// </summary>
        [JsonPropertyName("approvalEmplId")]
        public string ApprovalEmplId { get; set; }

        /// <summary>
        /// Gets or sets the apply date (yyyy-MM-dd without time).
        /// </summary>
        [JsonPropertyName("applyDate")]
        public string ApplyDate { get; set; }

        /// <summary>
        /// Gets or sets the apply time (HH:mm:ss without date).
        /// </summary>
        [JsonPropertyName("applyTime")]
        public string ApplyTime { get; set; }

        /// <summary>
        /// Gets or sets the list of next node approvals.
        /// </summary>
        [JsonPropertyName("nextNodeApproveds")]
        public List<string> NextNodeApproveds { get; set; }

        /// <summary>
        /// Gets or sets the BPMN start conditions.
        /// </summary>
        [JsonPropertyName("bpmnStartConditions")]
        public BpmnStartConditionsVo BpmnStartConditions { get; set; }

        /// <summary>
        /// Gets or sets the event type enum.
        /// </summary>
        [JsonPropertyName("eventTypeEnum")]
        public EventTypeEnum EventTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets whether the process is an outside process (third-party system started process).
        /// </summary>
        [JsonPropertyName("isOutside")]
        public bool IsOutside { get; set; } = false;
        
    }
}
