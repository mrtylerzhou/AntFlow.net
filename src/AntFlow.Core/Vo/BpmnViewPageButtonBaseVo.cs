using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnViewPageButtonBaseVo
{
    [JsonPropertyName("viewPageStart")] public List<int> ViewPageStart { get; set; }

    [JsonPropertyName("viewPageOther")] public List<int> ViewPageOther { get; set; }

    // Ĭ�Ϲ��캯��

    // �������Ĺ��캯��
}