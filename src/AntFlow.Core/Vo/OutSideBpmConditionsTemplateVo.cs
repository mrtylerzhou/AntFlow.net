using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSideBpmConditionsTemplateVo
{
    /// <summary>
    ///     auto incr id
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    ///     business party's id
    /// </summary>
    [JsonPropertyName("businessPartyId")]
    public long BusinessPartyId { get; set; }

    /// <summary>
    ///     template mark
    /// </summary>
    [JsonPropertyName("templateMark")]
    public string TemplateMark { get; set; }

    /// <summary>
    ///     template name
    /// </summary>
    [JsonPropertyName("templateName")]
    public string TemplateName { get; set; }

    /// <summary>
    ///     template application id
    /// </summary>
    [JsonPropertyName("applicationId")]
    public int ApplicationId { get; set; }

    /// <summary>
    ///     remark
    /// </summary>
    [JsonPropertyName("remark")]
    public string Remark { get; set; }

    /// <summary>
    ///     0 for normal,1 for delete
    /// </summary>
    [JsonPropertyName("isDel")]
    public int IsDel { get; set; }

    /// <summary>
    ///     create user
    /// </summary>
    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }

    /// <summary>
    ///     create time
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     update user
    /// </summary>
    [JsonPropertyName("updateUser")]
    public string UpdateUser { get; set; }

    /// <summary>
    ///     update time
    /// </summary>
    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     create user id
    /// </summary>
    [JsonPropertyName("createUserId")]
    public string CreateUserId { get; set; }

    //===============>>ext fields<<===================

    /// <summary>
    ///     business party's mark
    /// </summary>
    [JsonPropertyName("businessPartyMark")]
    public string BusinessPartyMark { get; set; }

    /// <summary>
    ///     business party's name
    /// </summary>
    [JsonPropertyName("businessPartyName")]
    public string BusinessPartyName { get; set; }

    /// <summary>
    ///     create user name
    /// </summary>
    [JsonPropertyName("createUserName")]
    public string CreateUserName { get; set; }

    /// <summary>
    ///     application id (formCode)
    /// </summary>
    [JsonPropertyName("applicationFormCode")]
    public string ApplicationFormCode { get; set; }

    /// <summary>
    ///     template application name
    /// </summary>
    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; }

    /// <summary>
    ///     template list
    /// </summary>
    [JsonPropertyName("templates")]
    public List<OutSideBpmConditionsTemplateVo> Templates { get; set; }

    /// <summary>
    ///     whether it is used or not
    /// </summary>
    [JsonPropertyName("isUsed")]
    public bool IsUsed { get; set; }

    //===============>>query conditions<<===================

    /// <summary>
    ///     业务方ID列表查询条件
    /// </summary>
    [JsonPropertyName("businessPartyIds")]
    public List<long?> BusinessPartyIds { get; set; }
}