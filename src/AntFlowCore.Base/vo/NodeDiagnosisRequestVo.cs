using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 节点诊断请求 (POST /bpmnConf/diagnoseNode) 与 Java 版 NodeDiagnosisRequestVo 对齐
/// </summary>
public class NodeDiagnosisRequestVo
{
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }

    /// <summary>t_bpmn_node 主键 id</summary>
    [JsonPropertyName("nodeId")]
    public long NodeId { get; set; }

    /// <summary>用户选择: true=有此节点 / false=没有此节点 / null=未选择</summary>
    [JsonPropertyName("expectedPresent")]
    public bool? ExpectedPresent { get; set; }

    /// <summary>人员维度: 选中的审批人 id (不传则只做节点维度)</summary>
    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    /// <summary>人员维度预期: true=预期此审批人存在 / false=预期不存在</summary>
    [JsonPropertyName("expectedPersonPresent")]
    public bool? ExpectedPersonPresent { get; set; }
}
