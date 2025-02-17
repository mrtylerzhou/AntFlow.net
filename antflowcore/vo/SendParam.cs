using System;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class SendParam
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("node")]
        public string Node { get; set; }

        [JsonPropertyName("params")]
        public string Params { get; set; }

        [JsonPropertyName("urlParams")]
        public UrlParams UrlParams { get; set; }

        [JsonPropertyName("appUrl")]
        public string AppUrl { get; set; }
    }
    
}