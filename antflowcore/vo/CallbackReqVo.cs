using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class CallbackReqVo
    {
        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the business party mark.
        /// </summary>
        [JsonPropertyName("businessPartyMark")]
        public string BusinessPartyMark { get; set; }

        /// <summary>
        /// Gets or sets the form code.
        /// </summary>
        [JsonPropertyName("formCode")]
        public string FormCode { get; set; }

        /// <summary>
        /// Gets or sets the process number.
        /// </summary>
        [JsonPropertyName("processNum")]
        public string ProcessNum { get; set; }

        /// <summary>
        /// Gets or sets the business ID from the partner system.
        /// </summary>
        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Gets or sets the list of process records.
        /// </summary>
        [JsonPropertyName("processRecord")]
        public List<OutSideBpmAccessProcessRecordVo> ProcessRecord { get; set; }
    }
}