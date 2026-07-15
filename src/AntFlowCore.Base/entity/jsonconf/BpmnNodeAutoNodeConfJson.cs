using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

/// <summary>
/// Auto node configuration JSON.
/// Stores conditions that determine when the automatic action should be executed.
/// Reuses <see cref="BpmnNodeConditionsConfVueVo"/> for condition items — same structure as condition nodes.
/// </summary>
public class BpmnNodeAutoNodeConfJson
{
    /// <summary>
    /// Condition groups (outer list = groups, inner list = conditions within a group).
    /// </summary>
    [JsonPropertyName("conditionList")]
    public List<List<BpmnNodeConditionsConfVueVo>>? ConditionList { get; set; }

    /// <summary>
    /// Group relation: false = AND between groups, true = OR between groups
    /// </summary>
    [JsonPropertyName("groupRelation")]
    public bool? GroupRelation { get; set; }
}
