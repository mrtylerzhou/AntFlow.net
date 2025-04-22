using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process department.
    /// </summary>
    [Table(Name = "bpm_process_dept")]
    public class BpmProcessDept
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process code.
        /// </summary>
        [Column(Name = "process_code")]
        public string ProcessCode { get; set; }

        /// <summary>
        /// Process type.
        /// </summary>
        [Column(Name = "process_type")]
        public int ProcessType { get; set; }

        /// <summary>
        /// Process name.
        /// </summary>
        [Column(Name = "process_name")]
        public string ProcessName { get; set; }

        /// <summary>
        /// Department ID to which the process belongs.
        /// </summary>
        [Column(Name = "dep_id")]
        public long DeptId { get; set; }

        /// <summary>
        /// Process remark.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Create user ID.
        /// </summary>
        [Column(Name = "create_user")]
        public long CreateUser { get; set; }

        /// <summary>
        /// Update user ID.
        /// </summary>
        [Column(Name = "update_user")]
        public long UpdateUser { get; set; }

        /// <summary>
        /// Modify time.
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Is deleted (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Is for all (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_all")]
        public int IsAll { get; set; }
    }
}