using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程对比: 加签/减签/转办记录项 (GET /bpmnConf/compareEntrusts)
/// 来源 bpm_flowrun_entrust(表自带 node_id)。
/// 与 Java 版 ProcessCompareEntrustVo 对齐 (前端共享)。
/// 设计: .scratch/process-instance-compare-design.md §4.2
/// </summary>
public class ProcessCompareEntrustVo
{
    /// <summary>设计节点 id (t_bpmn_node.id, 与 getBpmVerifyInfoVos 的 nodeId 同口径)</summary>
    [JsonPropertyName("nodeId")]
    public string NodeId { get; set; }

    /// <summary>0/1=转办 2=加签 3=减签 4=表单关联刷新</summary>
    [JsonPropertyName("actionType")]
    public int? ActionType { get; set; }

    /// <summary>actionType 可读名称</summary>
    [JsonPropertyName("actionTypeName")]
    public string ActionTypeName { get; set; }

    /// <summary>原审批人 id</summary>
    [JsonPropertyName("originalId")]
    public string OriginalId { get; set; }

    /// <summary>原审批人姓名</summary>
    [JsonPropertyName("originalName")]
    public string OriginalName { get; set; }

    /// <summary>实际/被操作审批人 id</summary>
    [JsonPropertyName("actualId")]
    public string ActualId { get; set; }

    /// <summary>实际/被操作审批人姓名</summary>
    [JsonPropertyName("actualName")]
    public string ActualName { get; set; }
}
