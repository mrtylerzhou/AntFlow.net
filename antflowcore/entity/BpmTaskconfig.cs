using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents task configuration.
    /// </summary>
    [Table(Name = "bpm_taskconfig")]
    public class BpmTaskconfig
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process definition ID.
        /// </summary>
        [Column(Name = "proc_def_id_")]
        public string ProcDefId { get; set; }

        /// <summary>
        /// Task definition key.
        /// </summary>
        [Column(Name = "task_def_key_")]
        public string TaskDefKey { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        [Column(Name = "user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// User number.
        /// </summary>
        [Column(Name = "number")]
        public int Number { get; set; }
    }
}