using System.Text.Json;
using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

/// <summary>
/// Aggregate node-level configuration JSON for t_bpmn_node.
/// Contains all consolidated sub-configurations.
/// Consolidates 26 BPMN node sub-tables into JSON.
/// </summary>
public class BpmnNodeConfigJson
{
    /// <summary>
    /// Approver configuration (personnel, role, loop, level, hrbp, customize, udr, form-related, outside-access, business-table)
    /// </summary>
    [JsonPropertyName("approverConf")]
    public BpmnNodeApproverConfJson? ApproverConf { get; set; }

    /// <summary>
    /// Conditions configuration
    /// </summary>
    [JsonPropertyName("conditionsConf")]
    public BpmnNodeConditionsConfJson? ConditionsConf { get; set; }

    /// <summary>
    /// Button, label, sign-up, additional sign configuration
    /// </summary>
    [JsonPropertyName("buttonSignConf")]
    public BpmnNodeButtonSignConfJson? ButtonSignConf { get; set; }

    /// <summary>
    /// Template and reminder configuration
    /// </summary>
    [JsonPropertyName("templateConf")]
    public BpmnNodeTemplateConfJson? TemplateConf { get; set; }

    /// <summary>
    /// Low-code form field control configuration
    /// </summary>
    [JsonPropertyName("lowCodeConf")]
    public BpmnNodeLowCodeConfJson? LowCodeConf { get; set; }

    /// <summary>
    /// Back type for disagree action (migrated from bpm_process_node_back)
    /// </summary>
    [JsonPropertyName("backType")]
    public int? BackType { get; set; }

    /// <summary>
    /// Target node ID (design-time UUID) for disagree-back behavior.
    /// Used when backType is 4 or 5 to specify which node to return to.
    /// </summary>
    [JsonPropertyName("backToNodeId")]
    public string? BackToNodeId { get; set; }

    /// <summary>
    /// Auto-node style condition configuration. Used by condition-approve (nodeType=12)
    /// and condition-copy (nodeType=13) nodes. Persisted under JSON key "autoNodeConf"
    /// to align with the front-end (shared with Java version).
    /// </summary>
    [JsonPropertyName("autoNodeConf")]
    public AutoNodeConfJson? AutoNodeConf { get; set; }

    /// <summary>
    /// Draw-back button behavior type.
    /// 0=unrestricted(default), 1=back to prev node, 2=back to initiator(no return),
    /// 3=back to initiator(return to sender), 4=back to specified nodes(no return),
    /// 5=back to specified nodes(return to sender)
    /// </summary>
    [JsonPropertyName("drawBackType")]
    public int? DrawBackType { get; set; }

    /// <summary>
    /// Target node IDs (design-time UUIDs) for draw-back behavior.
    /// Used when drawBackType is 4 or 5.
    /// </summary>
    [JsonPropertyName("drawBackNodeIds")]
    public List<string>? DrawBackNodeIds { get; set; }

    /// <summary>
    /// Forward button behavior type.
    /// 0=forward to any future node, 1=forward to specified nodes
    /// </summary>
    [JsonPropertyName("forwardType")]
    public int? ForwardType { get; set; }

    /// <summary>
    /// Target node IDs (design-time UUIDs) for forward behavior.
    /// Used when forwardType is 1.
    /// </summary>
    [JsonPropertyName("forwardNodeIds")]
    public List<string>? ForwardNodeIds { get; set; }
}

/// <summary>
/// Condition configuration JSON for condition-approve / condition-copy nodes.
/// Mirrors the front-end autoNodeConf structure (conditionList + groupRelation).
/// </summary>
public class AutoNodeConfJson
{
    [JsonPropertyName("conditionList")]
    public List<List<JsonElement>>? ConditionList { get; set; }

    [JsonPropertyName("groupRelation")]
    public bool? GroupRelation { get; set; }
}

/// <summary>
/// Approver configuration JSON for a BPMN node.
/// Consolidates: t_bpmn_node_personnel_conf, t_bpmn_node_personnel_empl_conf,
/// t_bpmn_node_role_conf, t_bpmn_node_role_outside_emp_conf,
/// t_bpmn_node_loop_conf, t_bpmn_node_assign_level_conf,
/// t_bpmn_node_hrbp_conf, t_bpmn_node_customize_conf,
/// t_bpmn_node_udr_conf, t_bpmn_node_form_related_user_conf,
/// t_bpmn_node_out_side_access_conf, t_bpmn_node_business_table_conf
/// </summary>
public class BpmnNodeApproverConfJson
{
    [JsonPropertyName("personnelConf")]
    public ApproverPersonnelConf? PersonnelConf { get; set; }

    [JsonPropertyName("roleConfList")]
    public List<ApproverRoleConf>? RoleConfList { get; set; }

    [JsonPropertyName("loopConf")]
    public ApproverLoopConf? LoopConf { get; set; }

    [JsonPropertyName("assignLevelConf")]
    public ApproverAssignLevelConf? AssignLevelConf { get; set; }

    [JsonPropertyName("hrbpConf")]
    public ApproverHrbpConf? HrbpConf { get; set; }

    [JsonPropertyName("customizeConf")]
    public ApproverCustomizeConf? CustomizeConf { get; set; }

    [JsonPropertyName("udrConfList")]
    public List<ApproverUDRConf>? UdrConfList { get; set; }

    [JsonPropertyName("formRelatedUserConfList")]
    public List<ApproverFormRelatedUserConf>? FormRelatedUserConfList { get; set; }

    [JsonPropertyName("prevNodeRelatedUserConfList")]
    public List<ApproverPrevNodeRelatedUserConf>? PrevNodeRelatedUserConfList { get; set; }

    [JsonPropertyName("outSideAccessConf")]
    public ApproverOutSideAccessConf? OutSideAccessConf { get; set; }

    [JsonPropertyName("businessTableConf")]
    public ApproverBusinessTableConf? BusinessTableConf { get; set; }
}

public class ApproverPersonnelConf
{
    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }

    [JsonPropertyName("employees")]
    public List<ApproverEmployeeInfo>? Employees { get; set; }
}

public class ApproverEmployeeInfo
{
    [JsonPropertyName("emplId")]
    public string? EmplId { get; set; }

    [JsonPropertyName("emplName")]
    public string? EmplName { get; set; }
}

public class ApproverRoleConf
{
    [JsonPropertyName("roleId")]
    public string? RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }

    [JsonPropertyName("outsideEmployees")]
    public List<ApproverEmployeeInfo>? OutsideEmployees { get; set; }
}

public class ApproverLoopConf
{
    [JsonPropertyName("loopEndType")]
    public int? LoopEndType { get; set; }

    [JsonPropertyName("loopNumberPlies")]
    public int? LoopNumberPlies { get; set; }

    [JsonPropertyName("loopEndPerson")]
    public string? LoopEndPerson { get; set; }

    [JsonPropertyName("noparticipatingStaffIds")]
    public string? NoparticipatingStaffIds { get; set; }

    [JsonPropertyName("loopEndGrade")]
    public int? LoopEndGrade { get; set; }
}

public class ApproverAssignLevelConf
{
    [JsonPropertyName("assignLevelType")]
    public int? AssignLevelType { get; set; }

    [JsonPropertyName("assignLevelGrade")]
    public int? AssignLevelGrade { get; set; }
}

public class ApproverHrbpConf
{
    [JsonPropertyName("hrbpConfType")]
    public int? HrbpConfType { get; set; }
}

public class ApproverCustomizeConf
{
    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }
}

public class ApproverUDRConf
{
    [JsonPropertyName("valueJson")]
    public string? ValueJson { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }

    [JsonPropertyName("udrProperty")]
    public string? UdrProperty { get; set; }

    [JsonPropertyName("udrPropertyName")]
    public string? UdrPropertyName { get; set; }

    [JsonPropertyName("ext1")]
    public string? Ext1 { get; set; }

    [JsonPropertyName("ext2")]
    public string? Ext2 { get; set; }

    [JsonPropertyName("ext3")]
    public string? Ext3 { get; set; }

    [JsonPropertyName("ext4")]
    public string? Ext4 { get; set; }
}

public class ApproverFormRelatedUserConf
{
    [JsonPropertyName("valueJson")]
    public string? ValueJson { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }

    [JsonPropertyName("valueType")]
    public int? ValueType { get; set; }

    [JsonPropertyName("valueTypeName")]
    public string? ValueTypeName { get; set; }
}

public class ApproverPrevNodeRelatedUserConf
{
    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }

    [JsonPropertyName("valueType")]
    public int? ValueType { get; set; }

    [JsonPropertyName("valueTypeName")]
    public string? ValueTypeName { get; set; }
}

public class ApproverOutSideAccessConf
{
    [JsonPropertyName("nodeMark")]
    public string? NodeMark { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }
}

public class ApproverBusinessTableConf
{
    [JsonPropertyName("configurationTableType")]
    public int? ConfigurationTableType { get; set; }

    [JsonPropertyName("tableFieldType")]
    public int? TableFieldType { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }
}

/// <summary>
/// Button, label, sign-up, and additional sign configuration JSON for a BPMN node.
/// Consolidates: t_bpmn_node_button_conf, t_bpmn_node_sign_up_conf,
/// t_bpmn_node_labels, t_bpmn_node_additional_sign_conf
/// </summary>
public class BpmnNodeButtonSignConfJson
{
    [JsonPropertyName("buttonConfList")]
    public List<ButtonSignButtonConf>? ButtonConfList { get; set; }

    [JsonPropertyName("signUpConf")]
    public ButtonSignSignUpConf? SignUpConf { get; set; }

    [JsonPropertyName("labels")]
    public List<ButtonSignNodeLabel>? Labels { get; set; }

    [JsonPropertyName("additionalSignConfList")]
    public List<ButtonSignAdditionalSignConf>? AdditionalSignConfList { get; set; }

    [JsonPropertyName("operationTypes")]
    public List<int>? OperationTypes { get; set; }
}

public class ButtonSignButtonConf
{
    [JsonPropertyName("buttonPageType")]
    public int? ButtonPageType { get; set; }

    [JsonPropertyName("buttonType")]
    public int? ButtonType { get; set; }

    [JsonPropertyName("buttonName")]
    public string? ButtonName { get; set; }

    [JsonPropertyName("startPageOnly")]
    public int? StartPageOnly { get; set; }
}

public class ButtonSignSignUpConf
{
    [JsonPropertyName("afterSignUpWay")]
    public int? AfterSignUpWay { get; set; }

    [JsonPropertyName("signUpType")]
    public int? SignUpType { get; set; }
}

public class ButtonSignNodeLabel
{
    [JsonPropertyName("labelName")]
    public string? LabelName { get; set; }

    [JsonPropertyName("labelValue")]
    public string? LabelValue { get; set; }
}

public class ButtonSignAdditionalSignConf
{
    [JsonPropertyName("signInfos")]
    public string? SignInfos { get; set; }

    [JsonPropertyName("signProperty")]
    public int? SignProperty { get; set; }

    [JsonPropertyName("signPropertyType")]
    public int? SignPropertyType { get; set; }

    [JsonPropertyName("signType")]
    public int? SignType { get; set; }

    [JsonPropertyName("arbitrationRatio")]
    public int? ArbitrationRatio { get; set; }
}

/// <summary>
/// Template and reminder configuration JSON for a BPMN node.
/// Consolidates: t_bpmn_template (node_id non-null), t_bpmn_approve_remind,
/// bpm_process_node_overtime (overtime config)
/// </summary>
public class BpmnNodeTemplateConfJson
{
    [JsonPropertyName("templates")]
    public List<BpmnTemplateVo>? Templates { get; set; }

    [JsonPropertyName("approveRemind")]
    public BpmnApproveRemindVo? ApproveRemind { get; set; }

    [JsonPropertyName("overtimeConf")]
    public TemplateOvertimeConf? OvertimeConf { get; set; }
}

public class TemplateOvertimeConf
{
    [JsonPropertyName("noticeTime")]
    public int? NoticeTime { get; set; }

    [JsonPropertyName("noticeTypes")]
    public List<int>? NoticeTypes { get; set; }
}

/// <summary>
/// Low-code form field control configuration JSON for a BPMN node.
/// Consolidates: t_bpmn_node_lf_formdata_field_control
/// </summary>
public class BpmnNodeLowCodeConfJson
{
    [JsonPropertyName("fieldControls")]
    public List<LFFieldControlVO>? FieldControls { get; set; }
}
