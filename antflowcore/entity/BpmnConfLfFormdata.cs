using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN configuration form data entity.
    /// </summary>
    [Table(Name = "t_bpmn_conf_lf_formdata")]
    public class BpmnConfLfFormdata
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// BPMN configuration ID.
        /// </summary>
        [Column(Name = "bpmn_conf_id")]
        public long BpmnConfId { get; set; }

        /// <summary>
        /// Form data (in JSON format).
        /// </summary>
        [Column(Name = "formdata")]
        public string Formdata { get; set; }

        /// <summary>
        /// Delete flag (0 = false, 1 = true).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Created by user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user.
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