using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class BpmVerifyInfoVo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("runInfoId")]
        public string RunInfoId { get; set; }

        [JsonPropertyName("verifyUserId")]
        public string VerifyUserId { get; set; }

        [JsonPropertyName("verifyUserIds")]
        public List<string> VerifyUserIds { get; set; }

        [JsonPropertyName("verifyUserName")]
        public string VerifyUserName { get; set; }

        [JsonPropertyName("verifyStatus")]
        public int VerifyStatus { get; set; }

        [JsonPropertyName("verifyDesc")]
        public string VerifyDesc { get; set; }

        [JsonPropertyName("verifyDate")]
        public DateTime? VerifyDate { get; set; }

        [JsonPropertyName("taskName")]
        public string TaskName { get; set; }

        [JsonPropertyName("businessType")]
        public int BusinessType { get; set; }

        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        [JsonPropertyName("verifyStatusName")]
        public string VerifyStatusName { get; set; }

        [JsonPropertyName("originalId")]
        public string OriginalId { get; set; }

        [JsonPropertyName("originalName")]
        public string OriginalName { get; set; }

        [JsonPropertyName("processCode")]
        public string ProcessCode { get; set; }

        [JsonPropertyName("processCodeList")]
        public List<string> ProcessCodeList { get; set; }

        [JsonPropertyName("elementId")]
        public string ElementId { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }
    }
}