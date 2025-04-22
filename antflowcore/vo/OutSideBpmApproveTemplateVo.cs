namespace antflowcore.vo;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class OutSideBpmApproveTemplateVo
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("businessPartyId")]
    public long? BusinessPartyId { get; set; }

    [JsonPropertyName("applicationId")]
    public int? ApplicationId { get; set; }

    [JsonPropertyName("approveTypeId")]
    public int? ApproveTypeId { get; set; }

    [JsonPropertyName("approveTypeName")]
    public string ApproveTypeName { get; set; }

    [JsonPropertyName("apiClientId")]
    public string ApiClientId { get; set; }

    [JsonPropertyName("apiClientSecret")]
    public string ApiClientSecret { get; set; }

    [JsonPropertyName("apiToken")]
    public string ApiToken { get; set; }

    [JsonPropertyName("apiUrl")]
    public string ApiUrl { get; set; }

    [JsonPropertyName("remark")]
    public string Remark { get; set; }

    [JsonPropertyName("isDel")]
    public int? IsDel { get; set; }

    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }

    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    [JsonPropertyName("updateUser")]
    public string UpdateUser { get; set; }

    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    [JsonPropertyName("createUserId")]
    public string CreateUserId { get; set; }

    //===============>> ext fields <<===================

    [JsonPropertyName("businessPartyMark")]
    public string BusinessPartyMark { get; set; }

    [JsonPropertyName("businessPartyName")]
    public string BusinessPartyName { get; set; }

    [JsonPropertyName("createUserName")]
    public string CreateUserName { get; set; }

    [JsonPropertyName("applicationFormCode")]
    public string ApplicationFormCode { get; set; }

    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; }

    [JsonPropertyName("templates")]
    public List<OutSideBpmApproveTemplateVo> Templates { get; set; }

    [JsonPropertyName("isUsed")]
    public bool? IsUsed { get; set; }

    //===============>> query conditions <<===================

    [JsonPropertyName("businessPartyIds")]
    public List<long> BusinessPartyIds { get; set; }
}