using System.Text.Json.Serialization;

namespace AntFlowCore.Vo;

public class BpmProcessAppApplicationVo
{
    /// <summary>
    /// Gets or sets the ID of the application.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the business party ID.
    /// </summary>
    [JsonPropertyName("businessPartyId")]
    public long BusinessPartyId { get; set; }

    /// <summary>
    /// Gets or sets the business code, mainly for third party business, empty for central system.
    /// </summary>
    [JsonPropertyName("businessCode")]
    public string BusinessCode { get; set; }

    /// <summary>
    /// Gets or sets the business name.
    /// </summary>
    [JsonPropertyName("businessName")]
    public string BusinessName { get; set; }

    /// <summary>
    /// Gets or sets the access type (1-embedded; 2-api access).
    /// </summary>
    [JsonPropertyName("accessType")]
    public int? AccessType { get; set; }

    /// <summary>
    /// Gets or sets the application title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the application type (1:process 2:application 3:parent application).
    /// </summary>
    [JsonPropertyName("applyType")]
    public int? ApplyType { get; set; }

    /// <summary>
    /// Gets or sets the name of the application type.
    /// </summary>
    [JsonPropertyName("applyTypeName")]
    public string ApplyTypeName { get; set; }

    /// <summary>
    /// Gets or sets the PC icon URL.
    /// </summary>
    [JsonPropertyName("pcIcon")]
    public string PcIcon { get; set; }

    /// <summary>
    /// Gets or sets the effective source URL (app icon).
    /// </summary>
    [JsonPropertyName("effectiveSource")]
    public string EffectiveSource { get; set; }

    /// <summary>
    /// Gets or sets whether it is a child application (0:no 1:yes).
    /// </summary>
    [JsonPropertyName("isSon")]
    public int? IsSon { get; set; }

    /// <summary>
    /// Gets or sets the view URL.
    /// </summary>
    [JsonPropertyName("lookUrl")]
    public string LookUrl { get; set; }

    /// <summary>
    /// Gets or sets the submit URL.
    /// </summary>
    [JsonPropertyName("submitUrl")]
    public string SubmitUrl { get; set; }

    /// <summary>
    /// Gets or sets the condition URL.
    /// </summary>
    [JsonPropertyName("conditionUrl")]
    public string ConditionUrl { get; set; }

    /// <summary>
    /// Gets or sets the parent application ID.
    /// </summary>
    [JsonPropertyName("parentId")]
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the application URL.
    /// </summary>
    [JsonPropertyName("applicationUrl")]
    public string ApplicationUrl { get; set; }

    /// <summary>
    /// Gets or sets the route.
    /// </summary>
    [JsonPropertyName("route")]
    public string Route { get; set; }

    /// <summary>
    /// Gets or sets the process key.
    /// </summary>
    [JsonPropertyName("processKey")]
    public string ProcessKey { get; set; }

    /// <summary>
    /// Gets or sets the permission code.
    /// </summary>
    [JsonPropertyName("permissionsCode")]
    public string PermissionsCode { get; set; }

    /// <summary>
    /// Gets or sets whether it is deleted (0 for not deleted, 1 for deleted).
    /// </summary>
    [JsonPropertyName("isDel")]
    public int? IsDel { get; set; }

    /// <summary>
    /// Gets or sets the creator's user ID.
    /// </summary>
    [JsonPropertyName("createUserId")]
    public string CreateUserId { get; set; }

    /// <summary>
    /// Gets or sets the creation time.
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Gets or sets the updater's user ID.
    /// </summary>
    [JsonPropertyName("updateUser")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// Gets or sets the update time.
    /// </summary>
    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Gets or sets whether it is for all users (1 for yes).
    /// </summary>
    [JsonPropertyName("isAll")]
    public int? IsAll { get; set; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    [JsonPropertyName("sort")]
    public int? Sort { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the list of process keys.
    /// </summary>
    [JsonPropertyName("processKeyList")]
    public List<string> ProcessKeyList { get; set; }

    /// <summary>
    /// Gets or sets the list of process types.
    /// </summary>
    [JsonPropertyName("processTypes")]
    public List<long> ProcessTypes { get; set; }

    /// <summary>
    /// Gets or sets the list of process type names.
    /// </summary>
    [JsonPropertyName("processTypeNames")]
    public List<string> ProcessTypeNames { get; set; }

    /// <summary>
    /// Gets or sets the list of process categories.
    /// </summary>
    [JsonPropertyName("processTypeList")]
    public List<BpmProcessCategoryVo> ProcessTypeList { get; set; }

    /// <summary>
    /// Gets or sets the process name.
    /// </summary>
    [JsonPropertyName("processName")]
    public string ProcessName { get; set; }

    /// <summary>
    /// Gets or sets the process type name.
    /// </summary>
    [JsonPropertyName("processTypeName")]
    public string ProcessTypeName { get; set; }

    /// <summary>
    /// Gets or sets the type name.
    /// </summary>
    [JsonPropertyName("typeName")]
    public string TypeName { get; set; }

    /// <summary>
    /// Gets or sets the type IDs.
    /// </summary>
    [JsonPropertyName("typeIds")]
    public string TypeIds { get; set; }

    /// <summary>
    /// Gets or sets whether it is an app application.
    /// </summary>
    [JsonPropertyName("isApp")]
    public string IsApp { get; set; }

    /// <summary>
    /// Gets or sets the entrance URL.
    /// </summary>
    [JsonPropertyName("entrance")]
    public string Entrance { get; set; }

    /// <summary>
    /// Gets or sets the process code.
    /// </summary>
    [JsonPropertyName("processCode")]
    public string ProcessCode { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the visibility state.
    /// </summary>
    [JsonPropertyName("visbleState")]
    public int? VisbleState { get; set; }

    /// <summary>
    /// Gets or sets the creation time as a string.
    /// </summary>
    [JsonPropertyName("createTimeStr")]
    public string CreateTimeStr { get; set; }

    /// <summary>
    /// Gets or sets the search keyword.
    /// </summary>
    [JsonPropertyName("search")]
    public string Search { get; set; }

    /// <summary>
    /// Gets or sets the limit size.
    /// </summary>
    [JsonPropertyName("limitSize")]
    public int? LimitSize { get; set; }

    /// <summary>
    /// Gets or sets the list of business codes.
    /// </summary>
    [JsonPropertyName("businessCodeList")]
    public List<string> BusinessCodeList { get; set; }

    /// <summary>
    /// Gets or sets the creator's username.
    /// </summary>
    [JsonPropertyName("createUserName")]
    public string CreateUserName { get; set; }

    /// <summary>
    /// Gets or sets the process category ID.
    /// </summary>
    [JsonPropertyName("processCategoryId")]
    public int? ProcessCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the application ID.
    /// </summary>
    [JsonPropertyName("applicationId")]
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets whether it is a third party or central system application.
    /// </summary>
    [JsonPropertyName("isBusiness")]
    public int? IsBusiness { get; set; }

    /// <summary>
    /// Gets or sets the list of IDs.
    /// </summary>
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; }

    /// <summary>
    /// Gets or sets whether the application can be deleted.
    /// </summary>
    [JsonPropertyName("isCanDel")]
    public bool? IsCanDel { get; set; }
}