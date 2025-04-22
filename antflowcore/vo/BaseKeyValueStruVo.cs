using System.Text.Json.Serialization;

namespace antflowcore.vo;

public class BaseKeyValueStruVo
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("remark")]
    public string Remark { get; set; }

    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [JsonPropertyName("hasStarUserChooseModule")]
    public bool HasStarUserChooseModule { get; set; } = false;
}