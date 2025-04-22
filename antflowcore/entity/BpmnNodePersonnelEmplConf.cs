using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node personnel employees.
    /// </summary>
    [Table(Name = "t_bpmn_node_personnel_empl_conf")]
    public class BpmnNodePersonnelEmplConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// BPMN node personnel ID.
        /// </summary>
        [Column(Name = "bpmn_node_personne_id")]
        public int BpmnNodePersonneId { get; set; }

        /// <summary>
        /// Employee ID.
        /// </summary>
        [Column(Name = "empl_id")]
        public string EmplId { get; set; }

        /// <summary>
        /// Employee name.
        /// </summary>
        [Column(Name = "empl_name")]
        public string EmplName { get; set; }

        /// <summary>
        /// Remarks.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Created by user (email prefix).
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user (email prefix).
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