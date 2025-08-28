using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmProcessVo
{
    [JsonPropertyName("processName")] public string ProcessName { get; set; }

    [JsonPropertyName("processKey")] public string ProcessKey { get; set; }
}