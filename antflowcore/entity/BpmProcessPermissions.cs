using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents BPM process permissions.
    /// </summary>
    [Table(Name = "bpm_process_permissions")]
    public class BpmProcessPermissions
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        [Column(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Department ID.
        /// </summary>
        [Column(Name = "dep_id")]
        public long DepId { get; set; }

        /// <summary>
        /// Permission type:
        /// 1: View
        /// 2: Create
        /// 3: Monitor
        /// </summary>
        [Column(Name = "permissions_type")]
        public int PermissionsType { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Office ID.
        /// </summary>
        [Column(Name = "office_id")]
        public long OfficeId { get; set; }
    }
}