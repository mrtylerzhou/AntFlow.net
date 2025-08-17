using System;
using System.Text.Json.Serialization;
using AntFlowCore.Vo;

namespace AntFlowCore.Entity
{
    public class UserMessage
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("user_id")]
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

        [JsonPropertyName("is_read")]
        public bool IsRead { get; set; }

        [JsonPropertyName("is_del")]
        public bool IsDel { get; set; }
        [JsonPropertyName("tenant_id")]
        public string TenantId { get; set; }
        [JsonPropertyName("create_time")]
        public DateTime? CreateTime { get; set; }

        [JsonPropertyName("update_time")]
        public DateTime? UpdateTime { get; set; }

        [JsonPropertyName("create_user")]
        public string CreateUser { get; set; }

        [JsonPropertyName("update_user")]
        public string UpdateUser { get; set; }

        [JsonPropertyName("app_url")]
        public string AppUrl { get; set; }

        [JsonPropertyName("source")]
        public int Source { get; set; }
    }
}