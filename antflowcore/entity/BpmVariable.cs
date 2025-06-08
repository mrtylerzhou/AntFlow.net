using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents process variables.
    /// </summary>
    [Table(Name = "t_bpm_variable")]
    public class BpmVariable
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process number.
        /// </summary>
        [Column(Name = "process_num")]
        public string ProcessNum { get; set; }

        /// <summary>
        /// Process name.
        /// </summary>
        [Column(Name = "process_name")]
        public string ProcessName { get; set; }

        /// <summary>
        /// Process description.
        /// </summary>
        [Column(Name = "process_desc")]
        public string ProcessDesc { get; set; } = "";

        /// <summary>
        /// Process start conditions.
        /// </summary>
        [Column(Name = "process_start_conditions")]
        public string ProcessStartConditions { get; set; }

        /// <summary>
        /// BPMN code.
        /// </summary>
        [Column(Name = "bpmn_code")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// Indicates whether the record is deleted (0 for normal, 1 for delete).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        [Column(Name = "create_user")]
        public string? CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}
