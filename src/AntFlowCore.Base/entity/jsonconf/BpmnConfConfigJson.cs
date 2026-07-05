using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

public class BpmnConfConfigJson
{
    [JsonPropertyName("viewPageButtons")]
    public List<ViewPageButtonItem>? ViewPageButtons { get; set; }

    [JsonPropertyName("confTemplates")]
    public List<ConfTemplateConf>? ConfTemplates { get; set; }

    [JsonPropertyName("lowCodeFormConfig")]
    public LowCodeFormConfig? LowCodeFormConfig { get; set; }

    [JsonPropertyName("noticeTemplateConfig")]
    public NoticeTemplateConfig? NoticeTemplateConfig { get; set; }

    [JsonPropertyName("noticeChannelTypes")]
    public List<int>? NoticeChannelTypes { get; set; }
}

/// <summary>
/// 对应 Java 版 BpmnNodeTemplateConfJson.TemplateConf,存直接接收的原始数据而非计算后的逗号字符串。
/// </summary>
public class ConfTemplateConf
{
    /// <summary>Event type</summary>
    [JsonPropertyName("event")]
    public int Event { get; set; }

    /// <summary>Inform ID list (原始数据)</summary>
    [JsonPropertyName("informIdList")]
    public List<string>? InformIdList { get; set; }

    /// <summary>Employee list (原始数据)</summary>
    [JsonPropertyName("empList")]
    public List<BaseIdTranStruVo>? EmpList { get; set; }

    /// <summary>Role list (原始数据)</summary>
    [JsonPropertyName("roleList")]
    public List<BaseIdTranStruVo>? RoleList { get; set; }

    /// <summary>Function list (原始数据)</summary>
    [JsonPropertyName("funcList")]
    public List<BaseIdTranStruVo>? FuncList { get; set; }

    /// <summary>Message template ID</summary>
    [JsonPropertyName("templateId")]
    public long? TemplateId { get; set; }

    /// <summary>Message send type as string</summary>
    [JsonPropertyName("messageSendType")]
    public string? MessageSendType { get; set; }

    /// <summary>Form code</summary>
    [JsonPropertyName("formCode")]
    public string? FormCode { get; set; }
}

public class NoticeTemplateConfig
{
    [JsonPropertyName("details")]
    public List<NoticeTemplateDetailItem>? Details { get; set; }
}

public class NoticeTemplateDetailItem
{
    [JsonPropertyName("noticeTemplateType")]
    public int NoticeTemplateType { get; set; }

    [JsonPropertyName("noticeTemplateDetail")]
    public string? NoticeTemplateDetailContent { get; set; }
}

public class ViewPageButtonItem
{
    [JsonPropertyName("viewType")]
    public int ViewType { get; set; }

    [JsonPropertyName("buttonType")]
    public int ButtonType { get; set; }

    [JsonPropertyName("buttonName")]
    public string? ButtonName { get; set; }
}

public class LowCodeFormConfig
{
    [JsonPropertyName("formdata")]
    public string? Formdata { get; set; }

    [JsonPropertyName("fields")]
    public List<LowCodeFormField>? Fields { get; set; }
}

public class LowCodeFormField
{
    [JsonPropertyName("fieldId")]
    public string? FieldId { get; set; }

    [JsonPropertyName("fieldName")]
    public string? FieldName { get; set; }

    [JsonPropertyName("fieldType")]
    public int? FieldType { get; set; }

    [JsonPropertyName("isConditionField")]
    public int IsConditionField { get; set; }
}
