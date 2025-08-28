using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BaseNumIdStruVo
{
    // Ĭ�Ϲ��캯��
    public BaseNumIdStruVo() { }

    // �������Ĺ��캯��
    public BaseNumIdStruVo(long id, string name)
    {
        Id = id;
        Name = name;
    }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    public bool Active { get; set; }
}