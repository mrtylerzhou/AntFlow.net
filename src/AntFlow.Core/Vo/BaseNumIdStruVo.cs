using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BaseNumIdStruVo
{
    // 默认构造函数
    public BaseNumIdStruVo() { }

    // 带参数的构造函数
    public BaseNumIdStruVo(long id, string name)
    {
        Id = id;
        Name = name;
    }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    public bool Active { get; set; }
}