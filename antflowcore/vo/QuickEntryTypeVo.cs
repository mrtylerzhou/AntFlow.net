using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class QuickEntryTypeVo
    {
        /// <summary>
        /// Quick entry ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Quick entry ID
        /// </summary>
        [JsonPropertyName("quickEntryId")]
        public long QuickEntryId { get; set; }

        /// <summary>
        /// Type: 1 for PC, 2 for App
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Deletion status: 0 for normal, 1 for deleted
        /// </summary>
        [JsonPropertyName("isDel")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }
    }
}