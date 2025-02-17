using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// BPMN Node Sign Up Configuration
    /// </summary>
    [Table(Name = "t_bpmn_node_sign_up_conf")]
    public class BpmnNodeSignUpConf
    {
        /// <summary>
        /// Auto Increment ID
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Node ID
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long BpmnNodeId { get; set; }

        /// <summary>
        /// After Sign-Up Way (1: back to sign-up person, 2: not back to sign-up person)
        /// </summary>
        [Column(Name = "after_sign_up_way")]
        public int AfterSignUpWay { get; set; }

        /// <summary>
        /// Sign-Up Type (1: sequential, 2: unordered, 3: or)
        /// </summary>
        [Column(Name = "sign_up_type")]
        public int SignUpType { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion Status (0: normal, 1: deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update Time
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}