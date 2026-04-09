using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    public class BpmProcessVo
    {
        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("processKey")]
        public string ProcessKey { get; set; }
    }
}