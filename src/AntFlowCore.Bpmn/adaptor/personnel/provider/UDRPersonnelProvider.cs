using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

/// <summary>
/// User-Defined-Rule (UDR) personnel provider.
/// <para>
/// This is a demo/placeholder implementation. The nodeProperty is 17 (NODE_PROPERTY_ZDY_RULES).
/// The actual rule type is determined by <see cref="BpmnNodePropertysVo.UdrAssigneeProperty"/>
/// (a key-value pair, e.g. {id:"zdysp1", name:"自定义审批人1"}) which comes from the
/// dictionary (dict_type = "udr", see AFSpecialDictCategoryEnum).
/// </para>
/// <para>
/// <b>Extensibility:</b> Users should subclass this provider and override
/// <see cref="GetAssigneeList"/> (or the protected <see cref="QueryAssignees"/> method)
/// to implement their own custom approver logic. Register the subclass as a singleton
/// for <c>IBpmnPersonnelProviderService</c> and <c>UDRPersonnelProvider</c> in DI.
/// </para>
/// <para>
/// <see cref="BpmnNodePropertysVo.UdrValueJson"/> is optional: e.g. if the custom rule is
/// "the starter's leader", the starter is only known after the process starts, so no value
/// needs to be stored at design time — it can be taken from
/// <see cref="BpmnStartConditionsVo.StartUserId"/> at runtime.
/// </para>
/// </summary>
[NamedService(nameof(UDRPersonnelProvider))]
public class UDRPersonnelProvider : AbstractMissingAssignNodeAssigneeVoProvider
{
    private readonly ILogger<UDRPersonnelProvider> _logger;

    public UDRPersonnelProvider(
        AssigneeVoBuildUtils assigneeVoBuildUtils,
        IBpmnProcessAdminProvider processAdminProvider,
        ILogger<UDRPersonnelProvider> logger) : base(assigneeVoBuildUtils, processAdminProvider)
    {
        _logger = logger;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        BaseIdTranStruVo udrAssigneeProperty = property?.UdrAssigneeProperty;
        if (udrAssigneeProperty == null || string.IsNullOrEmpty(udrAssigneeProperty.Id))
        {
            throw new AFBizException("udrAssigneeProperty missing");
        }

        // optional: the value json stored at design time (e.g. user ids for "指定人员" rule)
        string udrValueJson = property.UdrValueJson;

        // The actual rule type, e.g. "zdysp1" represents "指定人员"
        string udrPropertyId = udrAssigneeProperty.Id;

        List<BaseIdTranStruVo> assignees = QueryAssignees(udrPropertyId, udrValueJson, bpmnNodeVo, startConditionsVo);

        return ProvideAssigneeList(bpmnNodeVo, assignees ?? new List<BaseIdTranStruVo>());
    }

    /// <summary>
    /// Query assignees based on the UDR property id and optional value json.
    /// <para>
    /// This is the demo implementation. Override this method in a subclass to implement
    /// your own custom approver logic. For example:
    /// <list type="bullet">
    /// <item>zdysp1 → 指定人员: deserialize udrValueJson to List&lt;string&gt; of user ids, then query users</item>
    /// <item>zdysp2 → 发起人的领导: use startConditionsVo.StartUserId to find the leader</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="udrPropertyId">the UDR rule type id (e.g. "zdysp1")</param>
    /// <param name="udrValueJson">optional value json stored at design time</param>
    /// <param name="bpmnNodeVo">the bpmn node vo</param>
    /// <param name="startConditionsVo">the start conditions vo</param>
    /// <returns>list of assignees</returns>
    protected virtual List<BaseIdTranStruVo> QueryAssignees(
        string udrPropertyId,
        string udrValueJson,
        BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        // DEMO: return a placeholder assignee. Replace with real logic in subclasses.
        _logger.LogWarning("UDRPersonnelProvider is using demo logic for udrPropertyId:{UdrPropertyId}. " +
                           "Override this provider to implement custom approver rules.", udrPropertyId);

        return new List<BaseIdTranStruVo>
        {
            new BaseIdTranStruVo("1", "张三")
        };
    }
}
