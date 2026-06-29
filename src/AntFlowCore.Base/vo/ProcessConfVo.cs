using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Core.vo;

/// <summary>
/// Process configuration VO (from Java ProcessConfVo)
/// </summary>
public class ProcessConfVo
{
    /// <summary>
    /// auto incr id
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// process code
    /// </summary>
    [JsonPropertyName("processCode")]
    public string? ProcessCode { get; set; }

    /// <summary>
    /// process type
    /// </summary>
    [JsonPropertyName("processType")]
    public int? ProcessType { get; set; }

    /// <summary>
    /// process type name
    /// </summary>
    [JsonPropertyName("processTypeName")]
    public string? ProcessTypeName { get; set; }

    /// <summary>
    /// process name
    /// </summary>
    [JsonPropertyName("processName")]
    public string? ProcessName { get; set; }

    /// <summary>
    /// process belonging department
    /// </summary>
    [JsonPropertyName("deptId")]
    public long? DeptId { get; set; }

    /// <summary>
    /// remarks
    /// </summary>
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    /// <summary>
    /// create time
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// create user
    /// </summary>
    [JsonPropertyName("createUser")]
    public long? CreateUser { get; set; }

    /// <summary>
    /// update user
    /// </summary>
    [JsonPropertyName("updateUser")]
    public long? UpdateUser { get; set; }

    /// <summary>
    /// update time
    /// </summary>
    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// process key (maps to formCode in DB bpmn_conf)
    /// </summary>
    [JsonPropertyName("processKey")]
    public string? ProcessKey { get; set; }

    /// <summary>
    /// department name
    /// </summary>
    [JsonPropertyName("deptName")]
    public string? DeptName { get; set; }

    /// <summary>
    /// monitoring department Id list
    /// </summary>
    [JsonPropertyName("controlDeptIdList")]
    public List<BaseIdTranStruVo>? ControlDeptIdList { get; set; }

    /// <summary>
    /// monitoring user list
    /// </summary>
    [JsonPropertyName("controlUserIdList")]
    public List<BaseIdTranStruVo>? ControlUserIdList { get; set; }

    /// <summary>
    /// monitoring department id list
    /// </summary>
    [JsonPropertyName("controlDeptIds")]
    public List<long>? ControlDeptIds { get; set; }

    /// <summary>
    /// monitoring user id list
    /// </summary>
    [JsonPropertyName("controlUserIds")]
    public List<string>? ControlUserIds { get; set; }

    /// <summary>
    /// department list for create permission
    /// </summary>
    [JsonPropertyName("createDeptList")]
    public List<BaseIdTranStruVo>? CreateDeptList { get; set; }

    /// <summary>
    /// department id list for create permission
    /// </summary>
    [JsonPropertyName("createDeptIds")]
    public List<long>? CreateDeptIds { get; set; }

    /// <summary>
    /// user list for create permission
    /// </summary>
    [JsonPropertyName("createUserList")]
    public List<BaseIdTranStruVo>? CreateUserList { get; set; }

    /// <summary>
    /// user id list for create permission
    /// </summary>
    [JsonPropertyName("createUserIds")]
    public List<string>? CreateUserIds { get; set; }

    /// <summary>
    /// notify type list (via BaseIdTranStruVo)
    /// </summary>
    [JsonPropertyName("notifyTypeList")]
    public List<BaseIdTranStruVo>? NotifyTypeList { get; set; }

    /// <summary>
    /// notify type id list
    /// </summary>
    [JsonPropertyName("notifyTypeIds")]
    public List<int>? NotifyTypeIds { get; set; }

    /// <summary>
    /// remind type ids
    /// </summary>
    [JsonPropertyName("remindTypeIds")]
    public List<int>? RemindTypeIds { get; set; }

    /// <summary>
    /// remind type list
    /// </summary>
    [JsonPropertyName("remindTypeList")]
    public List<BaseIdTranStruVo>? RemindTypeList { get; set; }

    /// <summary>
    /// user id list for view permission
    /// </summary>
    [JsonPropertyName("viewUserIds")]
    public List<string>? ViewUserIds { get; set; }

    /// <summary>
    /// user list for view permission
    /// </summary>
    [JsonPropertyName("viewUserList")]
    public List<BaseIdTranStruVo>? ViewUserList { get; set; }

    /// <summary>
    /// depart id list for view permission
    /// </summary>
    [JsonPropertyName("viewdeptIds")]
    public List<long>? ViewdeptIds { get; set; }

    /// <summary>
    /// department list for view permission
    /// </summary>
    [JsonPropertyName("viewdeptList")]
    public List<BaseIdTranStruVo>? ViewdeptList { get; set; }

    /// <summary>
    /// node ids
    /// </summary>
    [JsonPropertyName("nodeIds")]
    public List<string>? NodeIds { get; set; }

    /// <summary>
    /// time out notice time
    /// </summary>
    [JsonPropertyName("noticeTime")]
    public int? NoticeTime { get; set; }

    /// <summary>
    /// process node vo list
    /// </summary>
    [JsonPropertyName("processNodeList")]
    public List<ProcessNodeVo>? ProcessNodeList { get; set; }

    /// <summary>
    /// process name and number fuzzy search
    /// </summary>
    [JsonPropertyName("search")]
    public string? Search { get; set; }

    /// <summary>
    /// is for all users
    /// </summary>
    [JsonPropertyName("isAll")]
    public int? IsAll { get; set; }

    /// <summary>
    /// icon id
    /// </summary>
    [JsonPropertyName("iconId")]
    public int? IconId { get; set; }

    /// <summary>
    /// create office ids
    /// </summary>
    [JsonPropertyName("createOfficeIds")]
    public List<long>? CreateOfficeIds { get; set; }

    /// <summary>
    /// create office list
    /// </summary>
    [JsonPropertyName("createOfficeList")]
    public List<BaseIdTranStruVo>? CreateOfficeList { get; set; }

    /// <summary>
    /// view office ids
    /// </summary>
    [JsonPropertyName("viewOfficeIds")]
    public List<long>? ViewOfficeIds { get; set; }

    /// <summary>
    /// view office list
    /// </summary>
    [JsonPropertyName("viewOfficeList")]
    public List<BaseIdTranStruVo>? ViewOfficeList { get; set; }

    /// <summary>
    /// template vos
    /// </summary>
    [JsonPropertyName("templateVos")]
    public List<BpmnTemplateVo>? TemplateVos { get; set; }
}
