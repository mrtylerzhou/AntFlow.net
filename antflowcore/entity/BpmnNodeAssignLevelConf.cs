using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN node assignment level configuration entity.
    /// </summary>
    [Table(Name = "t_bpmn_node_assign_level_conf")]
    public class BpmnNodeAssignLevelConf
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
        /// Assignment level type.
        /// </summary>
        [Column(Name = "assign_level_type")]
        public int? AssignLevelType { get; set; }

        /// <summary>
        /// Specified level grade.
        /// </summary>
        [Column(Name = "assign_level_grade")]
        public int AssignLevelGrade { get; set; }

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