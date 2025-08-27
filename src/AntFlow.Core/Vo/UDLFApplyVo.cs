using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class UDLFApplyVo : BusinessDataVo
{
    [JsonPropertyName("remark")] public string Remark { get; set; }

    [JsonPropertyName("lfFields")] public Dictionary<string, object> LfFields { get; set; }

    [JsonPropertyName("lfFormData")] public string LfFormData { get; set; }
}