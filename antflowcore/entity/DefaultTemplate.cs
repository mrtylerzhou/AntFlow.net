using System;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the default template entity.
    /// </summary>
    public class DefaultTemplate
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Event.
        /// </summary>
        public int Event { get; set; }

        /// <summary>
        /// Template ID.
        /// </summary>
        public long? TemplateId { get; set; }

        /// <summary>
        /// Indicates whether the template is deleted (0 - no, 1 - yes).
        /// </summary>
        public int IsDel { get; set; }
        public string TenantId { get; set; }
        /// <summary>
        /// Creation time.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Creator of the template.
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who updated the template.
        /// </summary>
        public string UpdateUser { get; set; }
    }
}