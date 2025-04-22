using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class ActivitiBpmMsgVo
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("process_id")]
        public string ProcessId { get; set; }

        [JsonPropertyName("bpmn_code")]
        public string BpmnCode { get; set; }

        [JsonPropertyName("form_code")]
        public string FormCode { get; set; }

        [JsonPropertyName("process_type")]
        public string ProcessType { get; set; }

        [JsonPropertyName("process_name")]
        public string ProcessName { get; set; }

        [JsonPropertyName("other_user_id")]
        public string OtherUserId { get; set; }

        [JsonPropertyName("cc")]
        public string[] Cc { get; set; }

        [JsonPropertyName("email_url")]
        public string EmailUrl { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("app_push_url")]
        public string AppPushUrl { get; set; }

        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }
    }
}