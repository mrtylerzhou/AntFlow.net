using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for external employees in BPMN node roles.
    /// </summary>
    [Table(Name = "t_bpmn_node_role_outside_emp_conf")]
    public class BpmnNodeRoleOutsideEmpConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Node ID.
        /// </summary>
        [Column(Name = "node_id")]
        public long NodeId { get; set; }

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
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user (email prefix).
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