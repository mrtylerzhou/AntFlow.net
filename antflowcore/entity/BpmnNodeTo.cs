using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN node "to" configuration.
    /// </summary>
    [Table(Name = "t_bpmn_node_to")]
    public class BpmnNodeTo
    {
        /// <summary>
        /// Auto-incrementing ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// BPMN node ID.
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long BpmnNodeId { get; set; }

        /// <summary>
        /// Node "to" value.
        /// </summary>
        [Column(Name = "node_to")]
        public string NodeTo { get; set; }

        /// <summary>
        /// Remark or additional information.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion status (0 for normal, 1 for delete).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// User who created this record.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Time when this record was created.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// User who last updated this record.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Time when this record was last updated.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}