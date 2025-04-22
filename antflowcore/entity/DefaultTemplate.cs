using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the default template entity.
    /// </summary>
    [Table(Name = "t_default_template")]
    public class DefaultTemplate
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Event.
        /// </summary>
        public int Event { get; set; }

        /// <summary>
        /// Template ID.
        /// </summary>
        [Column(Name = "template_id")]
        public long? TemplateId { get; set; }

        /// <summary>
        /// Indicates whether the template is deleted (0 - no, 1 - yes).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Creator of the template.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who updated the template.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }
}