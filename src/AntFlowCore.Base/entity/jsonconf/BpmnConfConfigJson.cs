using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

public class BpmnConfConfigJson
{
    [JsonPropertyName("viewPageButtons")]
    public List<ViewPageButtonItem>? ViewPageButtons { get; set; }

    [JsonPropertyName("confTemplates")]
    public List<BpmnTemplateVo>? ConfTemplates { get; set; }

    [JsonPropertyName("lowCodeFormConfig")]
    public LowCodeFormConfig? LowCodeFormConfig { get; set; }

    [JsonPropertyName("noticeTemplateConfig")]
    public NoticeTemplateConfig? NoticeTemplateConfig { get; set; }

    [JsonPropertyName("noticeChannelTypes")]
    public List<int>? NoticeChannelTypes { get; set; }
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
