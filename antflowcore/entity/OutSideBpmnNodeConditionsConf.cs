using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the node conditions configuration for the external BPM system.
    /// </summary>
    [Table(Name = "t_out_side_bpmn_node_conditions_conf")]
    public class OutSideBpmnNodeConditionsConf
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// BPM node ID
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long BpmnNodeId { get; set; }

        /// <summary>
        /// External conditions ID
        /// </summary>
        [Column(Name = "out_side_id")]
        public string OutSideId { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for deleted
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creator username
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updater username
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        // Default constructor for FreeSQL
        public OutSideBpmnNodeConditionsConf()
        { }
    }
}