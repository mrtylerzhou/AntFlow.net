using System.Text.Json.Serialization;
using AntFlowCore.Entities;
using antflowcore.entity;

namespace AntFlowCore.Vo
{
    public class BaseMsgInfo
    {
        [JsonPropertyName("msg_title")]
        public string MsgTitle { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("user_message_status")]
        public UserMessageStatus UserMessageStatus { get; set; }
    }
    
}