using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a sequence flow for a BPM variable.
    /// </summary>
    [Table(Name = "t_bpm_variable_sequence_flow")]
    public class BpmVariableSequenceFlow
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Variable ID.
        /// </summary>
        [Column(Name = "variable_id")]
        public long VariableId { get; set; }

        /// <summary>
        /// Element ID.
        /// </summary>
        [Column(Name = "element_id")]
        public string ElementId { get; set; }

        /// <summary>
        /// Element name.
        /// </summary>
        [Column(Name = "element_name")]
        public string ElementName { get; set; }

        /// <summary>
        /// Element start flow ID.
        /// </summary>
        [Column(Name = "element_from_id")]
        public string ElementFromId { get; set; }

        /// <summary>
        /// Element to ID (connected node).
        /// </summary>
        [Column(Name = "element_to_id")]
        public string ElementToId { get; set; }

        /// <summary>
        /// Flow type (1: no param, 2: has param).
        /// </summary>
        [Column(Name = "sequence_flow_type")]
        public int SequenceFlowType { get; set; }

        /// <summary>
        /// Flow conditions.
        /// </summary>
        [Column(Name = "sequence_flow_conditions")]
        public string SequenceFlowConditions { get; set; } = "";

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 0 for normal, 1 for delete.
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}