using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a flowrun entrust entity in BPMN.
    /// </summary>
    [Table(Name = "bpm_flowrun_entrust")]
    public class BpmFlowrunEntrust
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Process instance ID (not the task ID).
        /// </summary>
        [Column(Name = "runinfoid")]
        public string RunInfoId { get; set; }

        /// <summary>
        /// Current running task ID.
        /// </summary>
        [Column(Name = "runtaskid")]
        public string RunTaskId { get; set; }

        /// <summary>
        /// Original assignee.
        /// </summary>
        [Column(Name = "original")]
        public string Original { get; set; }

        /// <summary>
        /// Original assignee name.
        /// </summary>
        [Column(Name = "original_name")]
        public string OriginalName { get; set; }

        /// <summary>
        /// Actual assignee.
        /// </summary>
        [Column(Name = "actual")]
        public string Actual { get; set; }

        /// <summary>
        /// Actual assignee name.
        /// </summary>
        [Column(Name = "actual_name")]
        public string ActualName { get; set; }

        /// <summary>
        /// Type of task (1 for entrust task, 2 for circulate task).
        /// </summary>
        [Column(Name = "type")]
        public int Type { get; set; }

        /// <summary>
        /// Whether the task is read (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_read")]
        public int IsRead { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "proc_def_id")]
        public string ProcDefId { get; set; }

        /// <summary>
        /// Whether the task is viewed (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_view")]
        public int IsView { get; set; }
    }
}
