using System.Text.Json.Serialization;
using AntFlowCore.Base.util;

namespace AntFlowCore.Base.vo;

public class UDLFApplyVo: BusinessDataVo
{
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("lfFields")]
    public Dictionary<String,Object> LfFields { get; set; }
    [JsonPropertyName("lfFormData")]
    public String LfFormData { get; set; }
}