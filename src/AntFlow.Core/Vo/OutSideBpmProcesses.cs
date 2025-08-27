using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSideBpmProcesses
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("formCode")] public string FormCode { get; set; }
}