using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node roles.
    /// </summary>
    [Table(Name = "t_bpmn_node_role_conf")]
    public class BpmnNodeRoleConf
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
        /// Role ID.
        /// </summary>
        [Column(Name = "role_id")]
        public string RoleId { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        [Column(Name = "role_name")]
        public string RoleName { get; set; }

        /// <summary>
        /// Sign type (1 for all sign, 2 for OR sign).
        /// </summary>
        [Column(Name = "sign_type")]
        public int? SignType { get; set; }

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
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user (email prefix).
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}
