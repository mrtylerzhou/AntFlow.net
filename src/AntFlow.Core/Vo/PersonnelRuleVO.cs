using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class PersonnelRuleVo
{
    [JsonPropertyName("nodePropertyName")] public string NodePropertyName { get; set; }

    [JsonPropertyName("nodeProperty")]
    public int? NodeProperty { get; set; } // Using nullable int to handle possible null values

    [JsonPropertyName("fieldInfos")] public List<FieldAttributeInfoVO> FieldInfos { get; set; }
}