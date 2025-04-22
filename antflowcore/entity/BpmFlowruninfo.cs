using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the flowrun information entity in BPMN.
    /// </summary>
    [Table(Name = "bpm_flowruninfo")]
    public class BpmFlowruninfo
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        [Column(Name = "runinfoid")]
        public long RunInfoId { get; set; }

        /// <summary>
        /// Create user ID.
        /// </summary>
        [Column(Name = "create_UserId")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// Entity key.
        /// </summary>
        [Column(Name = "entitykey")]
        public string EntityKey { get; set; }

        /// <summary>
        /// Entity class.
        /// </summary>
        [Column(Name = "entityclass")]
        public string EntityClass { get; set; }

        /// <summary>
        /// Entity key type.
        /// </summary>
        [Column(Name = "entitykeytype")]
        public string EntityKeyType { get; set; }

        /// <summary>
        /// Created by.
        /// </summary>
        [Column(Name = "createactor")]
        public string CreateActor { get; set; }

        /// <summary>
        /// Creator department.
        /// </summary>
        [Column(Name = "createdepart")]
        public string CreateDepart { get; set; }

        /// <summary>
        /// Creation date.
        /// </summary>
        [Column(Name = "createdate")]
        public DateTime? CreateDate { get; set; }
    }
}