using AntFlow.Core.Conf.Json;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BaseIdTranStruVo
{
    // 默认构造函数
    public BaseIdTranStruVo() { }

    // 带参数的构造函数
    public BaseIdTranStruVo(string id, string name)
    {
        Id = id;
        Name = name;
    }

    [JsonPropertyName("id")]
    [JsonConverter(typeof(IntToStringConverter))]
    public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}