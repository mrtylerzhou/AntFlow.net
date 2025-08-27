using AntFlow.Core.Conf.Json;
using AntFlow.Core.Constant.Enums;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnNodeConditionsConfVueVo
{
    [JsonPropertyName("showType")] public string ShowType { get; set; }

    [JsonPropertyName("columnId")] public string ColumnId { get; set; }

    [JsonPropertyName("formId")] public int? FormId { get; set; }

    [JsonPropertyName("type")] public int? Type { get; set; }

    [JsonPropertyName("showName")] public string ShowName { get; set; }

    [JsonPropertyName("optType")]
    [JsonConverter(typeof(StringToIntConverter))]
    public int? OptType { get; set; }

    [JsonPropertyName("zdy1")] public string Zdy1 { get; set; }

    [JsonPropertyName("opt1")] public string Opt1 { get; set; }

    [JsonPropertyName("zdy2")] public string Zdy2 { get; set; }

    [JsonPropertyName("opt2")] public string Opt2 { get; set; }

    [JsonPropertyName("columnDbname")] public string ColumnDbname { get; set; }

    [JsonPropertyName("fieldTypeName")] public string FieldTypeName { get; set; }

    [JsonPropertyName("columnType")] public string ColumnType { get; set; }

    [JsonPropertyName("multiple")] public bool? Multiple { get; set; }

    [JsonPropertyName("multipleLimit")] public int MultipleLimit { get; set; }

    [JsonPropertyName("fixedDownBoxValue")]
    public string FixedDownBoxValue { get; set; }

    /// <summary>
    ///     <see cref="ConditionRelationShipEnum" />
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("condRelation")]
    public bool CondRelation { get; set; } = false;

    [JsonPropertyName("condGroup")] public int CondGroup { get; set; }
}