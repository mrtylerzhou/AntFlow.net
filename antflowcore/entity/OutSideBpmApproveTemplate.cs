using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// Third party process service, condition template conf
/// </summary>
[Table(Name = "t_out_side_bpm_approve_template")]
public class OutSideBpmApproveTemplate
{
    /// <summary>
    /// Auto increment id
    /// </summary>
    [Column(IsIdentity = true, IsPrimary = true)]
    public long Id { get; set; }

    /// <summary>
    /// Business party Id
    /// </summary>
    [Column(Name = "business_party_id")]
    public long? BusinessPartyId { get; set; }

    /// <summary>
    /// Application id
    /// </summary>
    [Column(Name = "application_id")]
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Approve type id
    /// </summary>
    [Column(Name = "approve_type_id")]
    public int? ApproveTypeId { get; set; }

    /// <summary>
    /// Approve type name
    /// </summary>
    [Column(Name = "approve_type_name")]
    public string ApproveTypeName { get; set; }

    /// <summary>
    /// API client id
    /// </summary>
    [Column(Name = "api_client_id")]
    public string ApiClientId { get; set; }

    /// <summary>
    /// API client secret
    /// </summary>
    [Column(Name = "api_client_secret")]
    public string ApiClientSecret { get; set; }

    /// <summary>
    /// API token
    /// </summary>
    [Column(Name = "api_token")]
    public string ApiToken { get; set; }

    /// <summary>
    /// API URL
    /// </summary>
    [Column(Name = "api_url")]
    public string ApiUrl { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 0 for normal, 1 for delete
    /// </summary>
    [Column(Name = "is_del")]
    public int? IsDel { get; set; }

    /// <summary>
    /// Create user
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }

    /// <summary>
    /// Create time
    /// </summary>
    [Column(Name = "create_time")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update user
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Create user id
    /// </summary>
    [Column(Name = "create_user_id")]
    public string CreateUserId { get; set; }
}