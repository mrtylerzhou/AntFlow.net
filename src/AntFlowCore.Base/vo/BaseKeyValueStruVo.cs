using System.Text.Json.Serialization;
using AntFlowCore.Constants;
using AntFlowCore.Core.conf.json;
namespace AntFlowCore.Core.vo;

public class BaseKeyValueStruVo
{
    [JsonPropertyName("key"),JsonConverter(typeof(IntToStringConverter))] 
    public string Key { get; set; }
    [JsonPropertyName("value")] 
    public string Value { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("remark")] 
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("createTime")] 
    public DateTime CreateTime { get; set; }

    [JsonPropertyName("processNotices")]
    public List<BaseNumIdStruVo> ProcessNotices { get; set; }
    [JsonPropertyName("hasStarUserChooseModule")]
    public bool HasStarUserChooseModule { get; set; } = false;
}