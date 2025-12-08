using System.Text.Json.Serialization;
using AntFlowCore.Constants;
using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UDLFApplyVo: BusinessDataVo
{
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    [JsonPropertyName("lfFields")]
    public Dictionary<String,Object> LfFields { get; set; }
    [JsonPropertyName("lfFormData")]
    public String LfFormData { get; set; }
}