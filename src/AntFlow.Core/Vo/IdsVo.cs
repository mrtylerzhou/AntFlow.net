using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class IdsVo
{
    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("powerId")] public string PowerId { get; set; }
}