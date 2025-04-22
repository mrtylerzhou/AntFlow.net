using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a quick entry configuration.
    /// </summary>
    [Table(Name = "t_quick_entry")]
    public class QuickEntry
    {
        /// <summary>
        /// Constant value for variable URL flag (True)
        /// </summary>
        public const int VARIABLE_URL_FLAG_TRUE = 1;

        /// <summary>
        /// Constant value for variable URL flag (False)
        /// </summary>
        public const int VARIABLE_URL_FLAG_FALSE = 0;

        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Application name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Effective image URL
        /// </summary>
        [Column(Name = "effective_source")]
        public string EffectiveSource { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Request URL
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Sort order
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Status (0 for enabled, 1 for disabled)
        /// </summary>
        [Column(Name = "status")]
        public int Status { get; set; }

        /// <summary>
        /// Variable URL flag (0 for false, 1 for true)
        /// </summary>
        [Column(Name = "variable_url_flag")]
        public int VariableUrlFlag { get; set; }

        // Default constructor for FreeSQL
        public QuickEntry() { }
    }
}