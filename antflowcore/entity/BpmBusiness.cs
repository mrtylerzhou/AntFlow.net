using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the business entity in BPMN.
    /// </summary>
    [Table(Name = "bpm_business")]
    public class BpmBusiness
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Business ID.
        /// </summary>
        [Column(Name = "business_id")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Process code.
        /// </summary>
        [Column(Name = "process_code")]
        public string ProcessCode { get; set; }

        /// <summary>
        /// Creator's user name.
        /// </summary>
        [Column(Name = "create_user_name")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// Creator's user ID.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Logical delete flag (0: not deleted, 1: deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }
    }
}