using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 节点诊断结论 (POST /bpmnConf/diagnoseNode) 与 Java 版 NodeDiagnosisVo 对齐 (前端共享)
/// 归因短路矩阵: ①实际存在性(present) ②尚未到达(NOT_REACHED) ③条件分支横评(CONDITION_MISS)
/// ④减签跳过(SIGN_SKIP) ⑤兜底(UNKNOWN); 节点存在时附带人员维度(4.3)
/// </summary>
public class NodeDiagnosisVo
{
    [JsonPropertyName("present")]
    public bool Present { get; set; }

    [JsonPropertyName("expectationMismatch")]
    public bool? ExpectationMismatch { get; set; }

    /// <summary>EXISTS / NOT_REACHED / CONDITION_MISS / SIGN_SKIP / UNKNOWN</summary>
    [JsonPropertyName("conclusionType")]
    public string ConclusionType { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("nodeName")]
    public string NodeName { get; set; }

    [JsonPropertyName("currentNodeName")]
    public string CurrentNodeName { get; set; }

    [JsonPropertyName("currentNodeId")]
    public string CurrentNodeId { get; set; }

    [JsonPropertyName("entrustRecords")]
    public List<EntrustRecordVo> EntrustRecords { get; set; } = new();

    [JsonPropertyName("signupRecords")]
    public List<SignupRecordVo> SignupRecords { get; set; } = new();

    [JsonPropertyName("prevNodeHasAddApproval")]
    public bool PrevNodeHasAddApproval { get; set; }

    [JsonPropertyName("prevNodeName")]
    public string PrevNodeName { get; set; }

    [JsonPropertyName("branches")]
    public List<BranchEvaluation> Branches { get; set; } = new();

    [JsonPropertyName("rawTasks")]
    public List<RawTaskVo> RawTasks { get; set; } = new();

    // ============ 人员维度 (4.3) ============

    [JsonPropertyName("expectedApprovers")]
    public List<ApproverVo> ExpectedApprovers { get; set; } = new();

    [JsonPropertyName("ruleDesc")]
    public string RuleDesc { get; set; }

    [JsonPropertyName("actualApprovers")]
    public List<ApproverVo> ActualApprovers { get; set; } = new();

    [JsonPropertyName("personDiagnosis")]
    public PersonDiagnosisVo PersonDiagnosis { get; set; }

    public class EntrustRecordVo
    {
        [JsonPropertyName("actionType")]
        public int? ActionType { get; set; }
        [JsonPropertyName("actionTypeName")]
        public string ActionTypeName { get; set; }
        [JsonPropertyName("originalId")]
        public string OriginalId { get; set; }
        [JsonPropertyName("originalName")]
        public string OriginalName { get; set; }
        [JsonPropertyName("actualId")]
        public string ActualId { get; set; }
        [JsonPropertyName("actualName")]
        public string ActualName { get; set; }
        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }
    }

    public class SignupRecordVo
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("verifyDate")]
        public DateTime? VerifyDate { get; set; }
        [JsonPropertyName("verifyDesc")]
        public string VerifyDesc { get; set; }
        /// <summary>verify_info / sign_up_config</summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class BranchEvaluation
    {
        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }
        [JsonPropertyName("priority")]
        public int? Priority { get; set; }
        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
        [JsonPropertyName("hit")]
        public bool? Hit { get; set; }
        [JsonPropertyName("containsTarget")]
        public bool ContainsTarget { get; set; }
        [JsonPropertyName("conditions")]
        public List<ConditionItemResult> Conditions { get; set; } = new();
    }

    public class ConditionItemResult
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }
        [JsonPropertyName("fieldTypeName")]
        public string FieldTypeName { get; set; }
        [JsonPropertyName("opText")]
        public string OpText { get; set; }
        [JsonPropertyName("expectText")]
        public string ExpectText { get; set; }
        [JsonPropertyName("actualValue")]
        public string ActualValue { get; set; }
        [JsonPropertyName("pass")]
        public bool Pass { get; set; }
    }

    public class RawTaskVo
    {
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }
        [JsonPropertyName("taskName")]
        public string TaskName { get; set; }
        [JsonPropertyName("assigneeName")]
        public string AssigneeName { get; set; }
        [JsonPropertyName("startTime")]
        public DateTime? StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public DateTime? EndTime { get; set; }
        [JsonPropertyName("deleteReason")]
        public string DeleteReason { get; set; }
        [JsonPropertyName("nodeId")]
        public string NodeId { get; set; }
        /// <summary>ru / hi</summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class ApproverVo
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>+加签 / -减签 / *转办 / null 原配置</summary>
        [JsonPropertyName("mark")]
        public string Mark { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
    }

    public class PersonDiagnosisVo
    {
        [JsonPropertyName("personId")]
        public string PersonId { get; set; }
        [JsonPropertyName("personName")]
        public string PersonName { get; set; }
        [JsonPropertyName("presentPerson")]
        public bool PresentPerson { get; set; }
        [JsonPropertyName("expectationMismatch")]
        public bool ExpectationMismatch { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("inference")]
        public bool Inference { get; set; }
        [JsonPropertyName("inferenceNote")]
        public string InferenceNote { get; set; }
    }
}
