using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN template configuration.
    /// </summary>
    [Table(Name = "t_bpmn_template")]
    public class BpmnTemplate
    {
        /// <summary>
        /// Auto-incrementing ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Configuration ID.
        /// </summary>
        [Column(Name = "conf_id")]
        public long ConfId { get; set; }

        /// <summary>
        /// Node ID.
        /// </summary>
        [Column(Name = "node_id")]
        public long? NodeId { get; set; }

        /// <summary>
        /// Event type.
        /// </summary>
        public int Event { get; set; }

        /// <summary>
        /// Inform types.
        /// </summary>
        public string Informs { get; set; }

        /// <summary>
        /// Specified employees.
        /// </summary>
        public string Emps { get; set; }

        /// <summary>
        /// Specified roles.
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// Specified functions.
        /// </summary>
        public string Funcs { get; set; }

        /// <summary>
        /// Template ID.
        /// </summary>
        [Column(Name = "template_id")]
        public long TemplateId { get; set; }

        /// <summary>
        /// Deletion status (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// User who created this record.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who last updated this record.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }
}
