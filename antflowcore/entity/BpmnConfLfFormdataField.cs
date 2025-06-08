using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN configuration form data field entity.
    /// </summary>
    [Table(Name = "t_bpmn_conf_lf_formdata_field")]
    public class BpmnConfLfFormdataField
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
        /// Form data ID.
        /// </summary>
        [Column(Name = "formdata_id")]
        public long FormDataId { get; set; }

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
        /// Field type.
        /// </summary>
        [Column(Name = "field_type")]
        public int? FieldType { get; set; }

        /// <summary>
        /// Is condition field (0: No, 1: Yes).
        /// </summary>
        [Column(Name = "is_condition")]
        public int IsConditionField { get; set; }

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
