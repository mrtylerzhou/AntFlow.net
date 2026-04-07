using System;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class UrlParams 
    {
        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("nodeType")]
        public string NodeType { get; set; }

        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        [JsonPropertyName("newsId")]
        public string NewsId { get; set; }
    }
}