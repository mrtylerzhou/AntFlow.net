using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程诊断初始化数据 (GET /bpmnConf/diagnosisInit)
/// 与 Java 版 ProcessDiagnosisInitVo 对齐 (前端共享)
/// </summary>
public class ProcessDiagnosisInitVo
{
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }

    /// <summary>t_bpmn_conf.id, 前端用它调 detail 接口</summary>
    [JsonPropertyName("confId")]
    public long ConfId { get; set; }

    /// <summary>版本号 (= bpm_business_process.version = t_bpmn_conf.bpmn_code)</summary>
    [JsonPropertyName("bpmnCode")]
    public string BpmnCode { get; set; }

    [JsonPropertyName("formCode")]
    public string FormCode { get; set; }

    [JsonPropertyName("isLowCodeFlow")]
    public int? IsLowCodeFlow { get; set; }

    /// <summary>流程是否已结束</summary>
    [JsonPropertyName("processFinished")]
    public bool ProcessFinished { get; set; }

    /// <summary>发起人 id (bpm_business_process.create_user)</summary>
    [JsonPropertyName("initiatorUserId")]
    public string InitiatorUserId { get; set; }

    /// <summary>发起人姓名 (bpm_business_process.user_name)</summary>
    [JsonPropertyName("initiatorUserName")]
    public string InitiatorUserName { get; set; }

    /// <summary>当前业务表单值, 供调试预填与条件实际值展示</summary>
    [JsonPropertyName("formValues")]
    public Dictionary<string, object> FormValues { get; set; } = new();
}
