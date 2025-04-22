using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node conditions.
    /// </summary>
    [Table(Name = "t_bpmn_node_conditions_conf")]
    public class BpmnNodeConditionsConf
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
        /// Is default condition (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_default")]
        public int? IsDefault { get; set; }

        /// <summary>
        /// Priority for sorting.
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// Extra JSON data for Vue3.
        /// </summary>
        [Column(Name = "ext_json")]
        public string ExtJson { get; set; }

        /// <summary>
        /// Additional remarks.
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
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user.
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