using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process forward.
    /// </summary>
    [Table(Name = "bpm_process_forward")]
    public class BpmProcessForward
    {

        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Forward user ID.
        /// </summary>
        [Column(Name = "forward_user_id")]
        public string ForwardUserId { get; set; }

        /// <summary>
        /// Forward user's name.
        /// </summary>
        [Column(Name = "Forward_user_name")]
        public string ForwardUserName { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        [Column(Name = "processInstance_Id")]
        public string ProcessInstanceId { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Create user ID.
        /// </summary>
        [Column(Name = "create_user_id")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// Is deleted (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Is read (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_read")]
        public int IsRead { get; set; }

        /// <summary>
        /// Task ID.
        /// </summary>
        [Column(Name = "task_id")]
        public string TaskId { get; set; }

        /// <summary>
        /// Process number.
        /// </summary>
        [Column(Name = "process_number")]
        public string ProcessNumber { get; set; }
    }
}