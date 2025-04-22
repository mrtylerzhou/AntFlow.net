using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class MessageInfo
    {
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("receiver")]
        public string Receiver { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("sign")]
        public string Sign { get; set; }
    }
}