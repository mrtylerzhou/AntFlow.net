using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

public class BpmnViewPageButtonBaseVo
{
    [JsonPropertyName("viewPageStart")] public List<int> ViewPageStart { get; set; } = new List<int>();

    [JsonPropertyName("viewPageOther")]
    public List<int> ViewPageOther { get; set; }=new List<int>();

    // 默认构造函数
    public BpmnViewPageButtonBaseVo() { }

    // 带参数的构造函数
}