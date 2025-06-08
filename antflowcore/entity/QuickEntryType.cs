using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a quick entry type configuration.
    /// </summary>
    [Table(Name = "t_quick_entry_type")]
    public class QuickEntryType
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Quick entry ID
        /// </summary>
        [Column(Name = "quick_entry_id")]
        public long QuickEntryId { get; set; }

        /// <summary>
        /// Type (1 for PC, 2 for App)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Type name
        /// </summary>
        [Column(Name = "type_name")]
        public string TypeName { get; set; }

        // Default constructor for FreeSQL
        public QuickEntryType() { }
    }
}