using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class ThirdPartyAccountApplyVo : BusinessDataVo
{
    [JsonPropertyName("accountType")] public int AccountType { get; set; }

    [JsonPropertyName("accountOwnerName")] public string AccountOwnerName { get; set; }

    [JsonPropertyName("remark")] public string Remark { get; set; }
}