using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSideBpmAccessEmbedNodeVo
{
    [JsonPropertyName("nodeMark")] public string NodeMark { get; set; }

    [JsonPropertyName("nodeName")] public string NodeName { get; set; }

    [JsonPropertyName("assigneeIdList")] public List<long> AssigneeIdList { get; set; }
}