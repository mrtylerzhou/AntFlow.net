using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class ProcessActionButtonVo
    {
        [JsonPropertyName("buttonType")]
        public int? ButtonType { get; set; }

        [JsonPropertyName("show")]
        public int? Show { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("appShow")]
        public int? AppShow { get; set; }
    }
}