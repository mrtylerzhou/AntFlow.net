using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class IdsVo
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("powerId")]
        public string PowerId { get; set; }
    }
}