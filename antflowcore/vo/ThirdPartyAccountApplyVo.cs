using System.Text.Json.Serialization;
using AntFlowCore.Vo;

namespace antflowcore.vo;

public class ThirdPartyAccountApplyVo: BusinessDataVo
{
    [JsonPropertyName("accountType")]
    public int AccountType { get; set; }
    [JsonPropertyName("accountOwnerName")]
    public String AccountOwnerName { get; set; }
    [JsonPropertyName("remark")]
    public String Remark { get; set; }
}