using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 独立表单管理 VO
/// </summary>
public class LfFormManageVo
{
    /// <summary>
    /// 表单版本ID (t_bpmn_conf_lf_formdata.id)
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// 家族标识（同族各版本共享）
    /// </summary>
    [JsonPropertyName("formCode")]
    public string FormCode { get; set; }

    /// <summary>
    /// 表单显示名
    /// </summary>
    [JsonPropertyName("formName")]
    public string FormName { get; set; }

    /// <summary>
    /// 表单数据 JSON
    /// </summary>
    [JsonPropertyName("formdata")]
    public string Formdata { get; set; }

    /// <summary>
    /// 是否当前生效版本 0否 1是
    /// </summary>
    [JsonPropertyName("effectiveStatus")]
    public int? EffectiveStatus { get; set; }

    /// <summary>
    /// 模糊搜索关键字
    /// </summary>
    [JsonPropertyName("search")]
    public string Search { get; set; }

    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }

    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    [JsonPropertyName("updateUser")]
    public string UpdateUser { get; set; }

    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 该家族的版本总数（列表展示用）
    /// </summary>
    [JsonPropertyName("versionCount")]
    public int? VersionCount { get; set; }
}
