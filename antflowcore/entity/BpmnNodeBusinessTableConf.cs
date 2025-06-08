using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node business table.
    /// </summary>
    [Table(Name = "t_bpmn_node_business_table_conf")]
    public class BpmnNodeBusinessTableConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// BPMN node ID.
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long BpmnNodeId { get; set; }

        /// <summary>
        /// Configuration table type.
        /// </summary>
        [Column(Name = "configuration_table_type")]
        public int? ConfigurationTableType { get; set; }

        /// <summary>
        /// Table field type.
        /// </summary>
        [Column(Name = "table_field_type")]
        public int? TableFieldType { get; set; }

        /// <summary>
        /// Sign type (1 for all sign, 2 for or sign).
        /// </summary>
        [Column(Name = "sign_type")]
        public int? SignType { get; set; }

        /// <summary>
        /// Remark or additional description.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted).
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
