using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程对比: 候选实例项 (GET /bpmnConf/compareCandidates)
/// 限定与当前实例同 formCode(bpm_business_process.PROCESSINESS_KEY)。
/// 与 Java 版 ProcessCompareCandidateVo 对齐 (前端共享)。
/// 设计: .scratch/process-instance-compare-design.md §4.1
/// </summary>
public class ProcessCompareCandidateVo
{
    /// <summary>流程编号 (bpm_business_process.BUSINESS_NUMBER)</summary>
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }

    /// <summary>流程版本即 bpmnCode (bpm_business_process.VERSION)</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>发起人 id (create_user)</summary>
    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }

    /// <summary>发起人姓名</summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    /// <summary>发起时间</summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>流程状态: 1审批中 2审批通过 3作废 6审批拒绝</summary>
    [JsonPropertyName("processState")]
    public int ProcessState { get; set; }

    /// <summary>对应模板配置 id (t_bpmn_conf.id, 前端据此调 /bpmnConf/detail/{confId})</summary>
    [JsonPropertyName("confId")]
    public long? ConfId { get; set; }

    /// <summary>模板名称 (t_bpmn_conf.bpmn_name)</summary>
    [JsonPropertyName("bpmnName")]
    public string BpmnName { get; set; }
}
