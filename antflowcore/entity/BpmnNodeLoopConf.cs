using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for loop in BPMN node.
    /// </summary>
    [Table(Name = "t_bpmn_node_loop_conf")]
    public class BpmnNodeLoopConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Node ID.
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long? BpmnNodeId { get; set; }

        /// <summary>
        /// Loop end type (used for extensibility, can be ignored if not needed).
        /// </summary>
        [Column(Name = "loop_end_type")]
        public int? LoopEndType { get; set; }

        /// <summary>
        /// Number of loop levels.
        /// </summary>
        [Column(Name = "loop_number_plies")]
        public int? LoopNumberPlies { get; set; }

        /// <summary>
        /// End person.
        /// </summary>
        [Column(Name = "loop_end_person")]
        public string LoopEndPerson { get; set; }

        /// <summary>
        /// Staff IDs not participating in the loop.
        /// </summary>
        [Column(Name = "noparticipating_staff_ids")]
        public string NoparticipatingStaffIds { get; set; }

        /// <summary>
        /// End grade.
        /// </summary>
        [Column(Name = "loop_end_grade")]
        public int? LoopEndGrade { get; set; }

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
