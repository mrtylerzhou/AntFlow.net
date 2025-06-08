using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process node record.
    /// </summary>
    [Table(Name = "bpm_process_node_record")]
    public class BpmProcessNodeRecord
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        [Column(Name = "processInstance_id")]
        public string ProcessInstanceId { get; set; }

        /// <summary>
        /// Task ID.
        /// </summary>
        [Column(Name = "task_id")]
        public string TaskId { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}