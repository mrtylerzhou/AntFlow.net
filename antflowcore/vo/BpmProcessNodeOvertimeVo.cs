using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BpmProcessNodeOvertimeVo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("noticeType")]
        public int NoticeType { get; set; }

        [JsonPropertyName("nodeName")]
        public string NodeName { get; set; }

        [JsonPropertyName("nodeKey")]
        public string NodeKey { get; set; }

        [JsonPropertyName("processDepId")]
        public long ProcessDepId { get; set; }

        [JsonPropertyName("noticeTime")]
        public int NoticeTime { get; set; }
    }
}