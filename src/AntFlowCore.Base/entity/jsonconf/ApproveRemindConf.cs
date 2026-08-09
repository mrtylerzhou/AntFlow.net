using System.Text.Json.Serialization;

namespace AntFlowCore.Base.entity.jsonconf
{
    /// <summary>
    /// Approval reminder config JSON (node_config_json -> templateConf.approveRemind).
    /// JSON shape MUST stay aligned with Java BpmnNodeTemplateConfJson.ApproveRemindConf.
    /// </summary>
    public class ApproveRemindConf
    {
        [JsonPropertyName("templateId")]
        public long? TemplateId { get; set; }

        /// <summary>
        /// Reminder days after timeout (1~7), day 1 = first 24h after timeout
        /// </summary>
        [JsonPropertyName("days")]
        public List<int> Days { get; set; }

        /// <summary>
        /// Node standard time limit in minutes
        /// </summary>
        [JsonPropertyName("standardMinutes")]
        public int? StandardMinutes { get; set; }

        /// <summary>
        /// Notice channel codes (MessageSendTypeEnum, incl. IN_SITE=4); empty means in-site only
        /// </summary>
        [JsonPropertyName("noticeTypes")]
        public List<int> NoticeTypes { get; set; }
    }
}