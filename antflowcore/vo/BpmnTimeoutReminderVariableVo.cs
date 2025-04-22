using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AntFlowCore.Entity;

namespace AntFlowCore.Vo
{
    public class BpmnTimeoutReminderVariableVo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processNum")]
        public string ProcessNum { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processDesc")]
        public string ProcessDesc { get; set; }

        [JsonPropertyName("processStartConditions")]
        public string ProcessStartConditions { get; set; }

        [JsonPropertyName("bpmnCode")]
        public string BpmnCode { get; set; }

        [JsonPropertyName("processinessKey")]
        public string ProcessinessKey { get; set; }

        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        [JsonPropertyName("entryId")]
        public string EntryId { get; set; }

        [JsonPropertyName("bpmVariableApproveReminds")]
        public List<BpmVariableApproveRemind> BpmVariableApproveReminds { get; set; }

        [JsonPropertyName("bpmnName")]
        public string BpmnName { get; set; }

        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        [JsonPropertyName("startUser")]
        public string StartUser { get; set; }

        [JsonPropertyName("approvalEmpl")]
        public string ApprovalEmpl { get; set; }

        [JsonPropertyName("applyDate")]
        public string ApplyDate { get; set; }

        [JsonPropertyName("applyTime")]
        public string ApplyTime { get; set; }

        [JsonPropertyName("assignee")]
        public string Assignee { get; set; }
    }
}