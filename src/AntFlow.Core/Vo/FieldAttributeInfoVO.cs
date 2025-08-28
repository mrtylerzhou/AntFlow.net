using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class FieldAttributeInfoVO
{
    [JsonPropertyName("fieldName")] public string FieldName { get; set; }

    // 字段类型信息
    [JsonPropertyName("fieldType")] public string FieldType { get; set; }

    [JsonPropertyName("fieldLabel")] public string FieldLabel { get; set; }

    [JsonPropertyName("fieldValue")] public string FieldValue { get; set; }

    [JsonPropertyName("choiceMaxValue")] public int? ChoiceMaxValue { get; set; }

    [JsonPropertyName("isMultiChoice")] public bool IsMultiChoice { get; set; } = false;

    [JsonPropertyName("sort")] public int Sort { get; set; }

    [JsonPropertyName("value")] public object Value { get; set; }
}