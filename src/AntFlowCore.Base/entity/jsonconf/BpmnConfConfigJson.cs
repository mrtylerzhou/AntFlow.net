using System.Text.Json.Serialization;

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

public class ConfTemplateConf
{
    /// <summary>Event type</summary>
    [JsonPropertyName("event")]
    public int Event { get; set; }

    /// <summary>Comma-separated inform IDs</summary>
    [JsonPropertyName("informs")]
    public string? Informs { get; set; }

    /// <summary>Comma-separated employee IDs</summary>
    [JsonPropertyName("emps")]
    public string? Emps { get; set; }

    /// <summary>Comma-separated role IDs</summary>
    [JsonPropertyName("roles")]
    public string? Roles { get; set; }

    /// <summary>Comma-separated function IDs</summary>
    [JsonPropertyName("funcs")]
    public string? Funcs { get; set; }

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
