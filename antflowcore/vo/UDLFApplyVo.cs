using System.Text.Json.Serialization;
using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UDLFApplyVo: BusinessDataVo
{
    [JsonPropertyName("remark")]
    public String Remark { get; set; }
    [JsonPropertyName("lfFields")]
    public Dictionary<String,Object> LfFields { get; set; }
    [JsonPropertyName("lfFormData")]
    public String LfFormData { get; set; }
}