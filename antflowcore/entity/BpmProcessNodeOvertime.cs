using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process node overtime.
    /// </summary>
    [Table(Name = "bpm_process_node_overtime")]
    public class BpmProcessNodeOvertime
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Notice type (1: email, 2: SMS, 3: app).
        /// </summary>
        [Column(Name = "notice_type")]
        public int NoticeType { get; set; }

        /// <summary>
        /// Node name.
        /// </summary>
        [Column(Name = "node_name")]
        public string NodeName { get; set; }

        /// <summary>
        /// Node key.
        /// </summary>
        [Column(Name = "node_key")]
        public string NodeKey { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Notice time.
        /// </summary>
        [Column(Name = "notice_time")]
        public int NoticeTime { get; set; }
    }
}