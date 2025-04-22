using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class UserMsgVo
    {
        /// <summary>
        /// Receiver ID
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Receiver email
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Carbon copy array
        /// </summary>
        [JsonPropertyName("cc")]
        public string[] Cc { get; set; }

        /// <summary>
        /// Receiver mobile phone number
        /// </summary>
        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Message title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Email URL
        /// </summary>
        [JsonPropertyName("emailUrl")]
        public string EmailUrl { get; set; }

        /// <summary>
        /// System message box URL
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// App push URL
        /// </summary>
        [JsonPropertyName("appPushUrl")]
        public string AppPushUrl { get; set; }

        /// <summary>
        /// Task ID
        /// </summary>
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        /// <summary>
        /// SSO domain
        /// </summary>
        [JsonPropertyName("ssoSessionDomain")]
        public string SsoSessionDomain { get; set; }

        /// <summary>
        /// Message source system
        /// </summary>
        [JsonPropertyName("source")]
        public int Source { get; set; }

        // Constructor
        public UserMsgVo()
        { }
    }
}