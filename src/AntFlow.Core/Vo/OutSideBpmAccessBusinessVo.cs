using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSideBpmAccessBusinessVo
{
    /// <summary>
    ///     Gets or sets the auto increment id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    ///     Gets or sets the business party id.
    /// </summary>
    [JsonPropertyName("businessPartyId")]
    public long? BusinessPartyId { get; set; }

    /// <summary>
    ///     Gets or sets the BPMN config id.
    /// </summary>
    [JsonPropertyName("bpmnConfId")]
    public long? BpmnConfId { get; set; }

    /// <summary>
    ///     Gets or sets the form code.
    /// </summary>
    [JsonPropertyName("formCode")]
    public string FormCode { get; set; }

    /// <summary>
    ///     Gets or sets the process number.
    /// </summary>
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }

    /// <summary>
    ///     Gets or sets the form data for PC.
    /// </summary>
    [JsonPropertyName("formDataPc")]
    public string FormDataPc { get; set; }

    /// <summary>
    ///     Gets or sets the form data for app.
    /// </summary>
    [JsonPropertyName("formDataApp")]
    public string FormDataApp { get; set; }

    /// <summary>
    ///     Gets or sets the template mark.
    /// </summary>
    [JsonPropertyName("templateMark")]
    public string TemplateMark { get; set; }

    [JsonPropertyName("TemplateMarks")] public List<string> TemplateMarks { get; set; }

    /// <summary>
    ///     Gets or sets the start user id.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    /// <summary>
    ///     Gets or sets the approval username.
    /// </summary>
    [JsonPropertyName("approvalUsername")]
    public string ApprovalUsername { get; set; }

    /// <summary>
    ///     Gets or sets the remark.
    /// </summary>
    [JsonPropertyName("remark")]
    public string Remark { get; set; }

    /// <summary>
    ///     Gets or sets the deletion status (0 for normal, 1 for deleted).
    /// </summary>
    [JsonPropertyName("isDel")]
    public int IsDel { get; set; }

    /// <summary>
    ///     Gets or sets the create user.
    /// </summary>
    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }

    /// <summary>
    ///     Gets or sets the create time.
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Gets or sets the update user.
    /// </summary>
    [JsonPropertyName("updateUser")]
    public string UpdateUser { get; set; }

    /// <summary>
    ///     Gets or sets the update time.
    /// </summary>
    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    // ========================== Extended Fields =========================

    /// <summary>
    ///     Gets or sets the process break description.
    /// </summary>
    [JsonPropertyName("processBreakDesc")]
    public string ProcessBreakDesc { get; set; }

    /// <summary>
    ///     Gets or sets the user who broke this process.
    /// </summary>
    [JsonPropertyName("processBreakUserId")]
    public string ProcessBreakUserId { get; set; }

    /// <summary>
    ///     Gets or sets the business party mark.
    /// </summary>
    [JsonPropertyName("businessPartyMark")]
    public string BusinessPartyMark { get; set; }

    /// <summary>
    ///     Gets or sets the application id.
    /// </summary>
    [JsonPropertyName("applicationId")]
    public int ApplicationId { get; set; }

    /// <summary>
    ///     Gets or sets the outside type.
    /// </summary>
    [JsonPropertyName("outSideType")]
    public int OutSideType { get; set; }

    /// <summary>
    ///     Gets or sets the embedded nodes.
    /// </summary>
    [JsonPropertyName("embedNodes")]
    public List<OutSideBpmAccessEmbedNodeVo> EmbedNodes { get; set; }

    /// <summary>
    ///     Gets or sets the outside level nodes.
    /// </summary>
    [JsonPropertyName("outSideLevelNodes")]
    public List<OutSideLevelNodeVo> OutSideLevelNodes { get; set; }

    [JsonPropertyName("lfConditions")] public Dictionary<string, object> LfConditions { get; set; }

    [JsonPropertyName("isLowCodeFlow")] public bool IsLowCodeFlow { get; set; }

    [JsonPropertyName("lfFields")] public Dictionary<string, object> LfFields { get; set; }

    [JsonPropertyName("approversList")] public Dictionary<string, List<BaseIdTranStruVo>> ApproversList { get; set; }
}