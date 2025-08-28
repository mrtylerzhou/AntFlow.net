using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class DataVo
{
    [JsonPropertyName("ids")] public List<IdsVo> Ids { get; set; }

    [JsonPropertyName("sender")] public string Sender { get; set; }

    [JsonPropertyName("receiverId")] public string ReceiverId { get; set; }

    [JsonPropertyName("receiverName")] public string ReceiverName { get; set; }

    [JsonPropertyName("beginTime")] public DateTime? BeginTime { get; set; }

    [JsonPropertyName("endTime")] public DateTime? EndTime { get; set; }
}