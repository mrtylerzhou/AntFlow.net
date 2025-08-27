using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class EnumerateVo
{
    [JsonPropertyName("code")] public int Code { get; set; }

    [JsonPropertyName("desc")] public string Desc { get; set; }

    // 枚举类型标识
    [JsonPropertyName("isInNode")] public bool IsInNode { get; set; }

    // 状态值：1表示启用 4表示禁用 0表示删除
    [JsonPropertyName("nodeType")] public int NodeType { get; set; }

    // 枚举值描述
    [JsonPropertyName("vo")] public BaseIdTranStruVo Vo { get; set; }

    // 枚举值编码
    [JsonPropertyName("informIdList")] public List<int> InformIdList { get; set; }

    [JsonPropertyName("processCode")] public string ProcessCode { get; set; }
}