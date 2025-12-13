using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using antflowcore.conf.json;

namespace AntFlowCore.Vo
{
    public class DataVo{
        [JsonPropertyName("ids")]
        public List<IdsVo> Ids { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("receiverId")]
        public string ReceiverId { get; set; }

        [JsonPropertyName("receiverName")]
        public string ReceiverName { get; set; }

        [JsonPropertyName("beginTime"),JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? BeginTime { get; set; }

        [JsonPropertyName("endTime"),JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? EndTime { get; set; }
    }
    
}