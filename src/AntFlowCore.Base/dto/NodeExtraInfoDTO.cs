using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.dto;

/// <summary>
/// Extra info DTO serialized into task FormKey.
/// Carries node labels so that BpmnTaskListener can read them at task creation time.
/// </summary>
public class NodeExtraInfoDTO
{
    [JsonPropertyName("nodeLabelVOS")]
    public List<BpmnNodeLabelVO>? NodeLabelVOS { get; set; }
}
