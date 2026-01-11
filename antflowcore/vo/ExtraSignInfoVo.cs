using System.Text.Json.Serialization;

namespace antflowcore.vo;

public class ExtraSignInfoVo
{
    
    [JsonPropertyName("propertyType")]
    public int? PropertyType { get; set; }
   
    [JsonPropertyName("nodeProperty")]
    public int? NodeProperty { get; set; }

    [JsonPropertyName("signInfos")]
    public List<BaseIdTranStruVo> SignInfos { get; set; }
   
    [JsonPropertyName("otherSignInfos")]
    public List<BaseIdTranStruVo> OtherSignInfos { get; set; }
}