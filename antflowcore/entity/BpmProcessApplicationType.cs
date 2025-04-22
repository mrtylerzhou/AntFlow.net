using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the process application type.
    /// </summary>
    [Table(Name = "bpm_process_application_type")]
    public class BpmProcessApplicationType
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Application ID.
        /// </summary>
        [Column(Name = "application_id")]
        public long ApplicationId { get; set; }

        /// <summary>
        /// Category ID.
        /// </summary>
        [Column(Name = "category_id")]
        public long CategoryId { get; set; }

        /// <summary>
        /// Deletion state (0 for normal, 1 for deleted).
        /// </summary>
        public int IsDel { get; set; }

        /// <summary>
        /// Sort order.
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Frequently used state (0 for no, 1 for yes).
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// History ID.
        /// </summary>
        [Column(Name = "history_id")]
        public long HistoryId { get; set; }

        /// <summary>
        /// Visibility state (0 for hidden, 1 for visible).
        /// </summary>
        [Column(Name = "visble_state")]
        public int VisbleState { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Common use state.
        /// </summary>
        [Column(Name = "common_use_state")]
        public int CommonUseState { get; set; }
    }
}