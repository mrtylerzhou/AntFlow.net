using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN node form data field control.
    /// </summary>
    [Table(Name = "t_bpmn_node_lf_formdata_field_control")]
    public class BpmnNodeLfFormdataFieldControl
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Node ID.
        /// </summary>
        [Column(Name = "node_id")]
        public long NodeId { get; set; }

        /// <summary>
        /// Form data ID.
        /// </summary>
        [Column(Name = "formdata_id")]
        public long? FormdataId { get; set; }

        /// <summary>
        /// Field ID.
        /// </summary>
        [Column(Name = "field_id")]
        public string FieldId { get; set; }

        /// <summary>
        /// Field name.
        /// </summary>
        [Column(Name = "field_name")]
        public string FieldName { get; set; }

        /// <summary>
        /// Field permission.
        /// </summary>
        [Column(Name = "field_perm")]
        public string Perm { get; set; }

        /// <summary>
        /// Logical delete flag (0: not deleted, 1: deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creator.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}