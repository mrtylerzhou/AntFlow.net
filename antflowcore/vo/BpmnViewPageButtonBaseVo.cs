using System.Text.Json.Serialization;

namespace antflowcore.vo;

public class BpmnViewPageButtonBaseVo
{
    [JsonPropertyName("viewPageStart")]
    public List<int> ViewPageStart { get; set; }

    [JsonPropertyName("viewPageOther")]
    public List<int> ViewPageOther { get; set; }

    // 默认构造函数
    public BpmnViewPageButtonBaseVo()
    { }

    // 带参数的构造函数
}