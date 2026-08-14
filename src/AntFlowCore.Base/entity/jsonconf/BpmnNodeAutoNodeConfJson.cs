using System.Text.Json;
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

    /// <summary>
    /// 满足条件时处理动作: 0=默认complete(默认), 1=跳转至固定节点, 2=加批, 3=转办, 4=抄送
    /// </summary>
    [JsonPropertyName("satisfiedAction")]
    public int? SatisfiedAction { get; set; }

    /// <summary>
    /// 不满足条件时处理动作: 0=默认complete(默认), 1=结束流程, 2=退回指定节点(重新开始)
    /// </summary>
    [JsonPropertyName("unsatisfiedAction")]
    public int? UnsatisfiedAction { get; set; }

    /// <summary>
    /// 跳转目标节点ID(设计态nodeId UUID), 仅 SatisfiedAction=1 时有值, 单选1个
    /// </summary>
    [JsonPropertyName("forwardNodeIds")]
    public List<string>? ForwardNodeIds { get; set; }

    /// <summary>
    /// 不满足退回目标节点ID(设计态nodeId UUID), 仅 UnsatisfiedAction=2 时有值, 单选1个
    /// </summary>
    [JsonPropertyName("backToNodeId")]
    public string? BackToNodeId { get; set; }

    /// <summary>
    /// 加批规则子配置(仅 SatisfiedAction=2 时有值), 结构同条件自动加批. 强制 afterSignUpWay=2
    /// </summary>
    [JsonPropertyName("autoSignUpConf")]
    public JsonElement? AutoSignUpConf { get; set; }

    /// <summary>
    /// 转办目标人(仅 SatisfiedAction=3 时有值): {id, name}. 不complete(任务转人工)
    /// </summary>
    [JsonPropertyName("transferToUser")]
    public JsonElement? TransferToUser { get; set; }

    /// <summary>
    /// 抄送规则子配置(仅 SatisfiedAction=4 时有值), 结构同加批配置. 逐人写 BpmProcessForward
    /// </summary>
    [JsonPropertyName("autoCopyConf")]
    public JsonElement? AutoCopyConf { get; set; }
}
