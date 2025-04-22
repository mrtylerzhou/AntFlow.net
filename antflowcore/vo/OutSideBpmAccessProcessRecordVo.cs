using System;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class OutSideBpmAccessProcessRecordVo
    {
        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [JsonPropertyName("nodeName")]
        public string NodeName { get; set; }

        /// <summary>
        /// Gets or sets the approval time.
        /// </summary>
        [JsonPropertyName("approvalTime")]
        public string ApprovalTime { get; set; }

        /// <summary>
        /// Gets or sets the approval status.
        /// </summary>
        [JsonPropertyName("approvalStatus")]
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets the approval status name.
        /// </summary>
        [JsonPropertyName("approvalStatusName")]
        public string ApprovalStatusName { get; set; }

        /// <summary>
        /// Gets or sets the approval username.
        /// </summary>
        [JsonPropertyName("approvalUserName")]
        public string ApprovalUserName { get; set; }

        /// <summary>
        /// Gets or sets the approval user id.
        /// </summary>
        [JsonPropertyName("approvalUserId")]
        public string ApprovalUserId { get; set; }
    }
}