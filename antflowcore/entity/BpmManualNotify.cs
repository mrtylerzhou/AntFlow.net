using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the manual notification entity in BPMN.
    /// </summary>
    [Table(Name = "bpm_manual_notify")]
    public class BpmManualNotify
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Business ID.
        /// </summary>
        [Column(Name = "business_id")]
        public long BusinessId { get; set; }

        /// <summary>
        /// Process code.
        /// </summary>
        [Column(Name = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Last reminder time.
        /// </summary>
        [Column(Name = "last_time")]
        public DateTime? LastTime { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}