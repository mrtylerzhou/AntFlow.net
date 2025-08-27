using AntFlow.Core.Entity;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class SendInfo
{
    [JsonPropertyName("mail")] public MailInfo Mail { get; set; }

    [JsonPropertyName("messageInfo")] public MessageInfo MessageInfo { get; set; }

    [JsonPropertyName("userMessage")] public UserMessage UserMessage { get; set; }

    [JsonPropertyName("baseMsgInfo")] public BaseMsgInfo BaseMsgInfo { get; set; }
}