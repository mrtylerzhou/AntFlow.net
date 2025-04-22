using AntFlowCore.Vo;
using FreeSql.DataAnnotations;
using System.Text.Json.Serialization;

namespace AntFlowCore.Entity
{
    [Table(Name = "t_user_message")]
    public class UserMessage
    {
        /// <summary>
        /// User message ID
        /// </summary>
        [JsonPropertyName("id")]
        [Column(IsPrimary = true, IsIdentity = true)] // Primary key, auto-increment
        public long Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        [Column(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Message title
        /// </summary>
        [JsonPropertyName("title")]
        [Column(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        [JsonPropertyName("content")]
        [Column(Name = "content")]
        public string Content { get; set; }

        /// <summary>
        /// Send URL
        /// </summary>
        [JsonPropertyName("url")]
        [Column(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Node ID
        /// </summary>
        [JsonPropertyName("node")]
        [Column(Name = "node")]
        public string Node { get; set; }

        /// <summary>
        /// Parameters for the message
        /// </summary>
        [JsonPropertyName("params")]
        [Column(Name = "params")]
        public string Params { get; set; }

        /// <summary>
        /// URL parameters (non-persistent field)
        /// </summary>
        [JsonPropertyName("urlParams")]
        [Column(IsIgnore = true)] // Ignore this field in database mapping
        public UrlParams UrlParams { get; set; }

        /// <summary>
        /// Read status (0 for unread, 1 for read)
        /// </summary>
        [JsonPropertyName("is_read")]
        [Column(Name = "is_read")]
        public bool IsRead { get; set; }

        /// <summary>
        /// Deletion status (0 for not deleted, 1 for deleted)
        /// </summary>
        [JsonPropertyName("is_del")]
        [Column(Name = "is_del")]
        public bool IsDel { get; set; }

        /// <summary>
        /// Message creation time
        /// </summary>
        [JsonPropertyName("create_time")]
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Message update time
        /// </summary>
        [JsonPropertyName("update_time")]
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who created the message
        /// </summary>
        [JsonPropertyName("create_user")]
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// User who updated the message
        /// </summary>
        [JsonPropertyName("update_user")]
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// App URL
        /// </summary>
        [JsonPropertyName("app_url")]
        [Column(Name = "app_url")]
        public string AppUrl { get; set; }

        /// <summary>
        /// Message source (integer representation)
        /// </summary>
        [JsonPropertyName("source")]
        [Column(Name = "source")]
        public int Source { get; set; }
    }
}