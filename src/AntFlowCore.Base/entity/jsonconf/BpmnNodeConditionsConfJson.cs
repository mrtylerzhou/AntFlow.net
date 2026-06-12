using System.Text.Json.Serialization;

namespace AntFlowCore.Base.entity.jsonconf;

public class BpmnNodeConditionsConfJson
{
    [JsonPropertyName("conditionGroups")]
    public List<ConditionGroup>? ConditionGroups { get; set; }

    [JsonPropertyName("outSideConditionId")]
    public string? OutSideConditionId { get; set; }

    public class ConditionGroup
    {
        [JsonPropertyName("isDefault")]
        public int? IsDefault { get; set; }

        [JsonPropertyName("groupRelation")]
        public int? GroupRelation { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        [JsonPropertyName("extJson")]
        public string? ExtJson { get; set; }

        [JsonPropertyName("params")]
        public List<ConditionParam>? Params { get; set; }
    }

    public class ConditionParam
    {
        [JsonPropertyName("conditionParamType")]
        public int? ConditionParamType { get; set; }

        [JsonPropertyName("conditionParamName")]
        public string? ConditionParamName { get; set; }

        [JsonPropertyName("conditionParamJsom")]
        public string? ConditionParamJsom { get; set; }

        [JsonPropertyName("operator")]
        public int? Operator { get; set; }

        [JsonPropertyName("condRelation")]
        public int? CondRelation { get; set; }

        [JsonPropertyName("condGroup")]
        public int? CondGroup { get; set; }
    }
}
