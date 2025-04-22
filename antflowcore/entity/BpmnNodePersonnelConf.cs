using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node personnel.
    /// </summary>
    [Table(Name = "t_bpmn_node_personnel_conf")]
    public class BpmnNodePersonnelConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// BPMN node ID.
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public int BpmnNodeId { get; set; }

        /// <summary>
        /// Sign type: 1 for co-sign, 2 for or-sign.
        /// </summary>
        [Column(Name = "sign_type")]
        public int SignType { get; set; }

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