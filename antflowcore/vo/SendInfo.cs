using System;
using System.Text.Json.Serialization;
using AntFlowCore.Entity;

namespace AntFlowCore.Vo
{
    public class SendInfo
    {
        [JsonPropertyName("mail")]
        public MailInfo Mail { get; set; }

        [JsonPropertyName("messageInfo")]
        public MessageInfo MessageInfo { get; set; }

        [JsonPropertyName("userMessage")]
        public UserMessage UserMessage { get; set; }

        [JsonPropertyName("baseMsgInfo")]
        public BaseMsgInfo BaseMsgInfo { get; set; }
    }
}