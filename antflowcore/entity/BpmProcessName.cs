using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Process Name Entity
    /// </summary>
    [Table(Name = "bpm_process_name")]
    public class BpmProcessName
    {
        /// <summary>
        /// Auto-increment ID
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process Name
        /// </summary>
        [Column(Name = "process_name")]
        public string ProcessName { get; set; }

        /// <summary>
        /// Deletion Status (0: Normal, 1: Deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }
    }
}