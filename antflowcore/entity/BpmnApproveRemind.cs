using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// BPMN Approve Remind
    /// </summary>
    [Table(Name = "t_bpmn_approve_remind")]
    public class BpmnApproveRemind
    {
        /// <summary>
        /// ID (Auto Increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Configuration ID
        /// </summary>
        [Column(Name = "conf_id")]
        public long ConfId { get; set; }

        /// <summary>
        /// Node ID
        /// </summary>
        [Column(Name = "node_id")]
        public long NodeId { get; set; }

        /// <summary>
        /// Template ID
        /// </summary>
        [Column(Name = "template_id")]
        public long? TemplateId { get; set; }

        /// <summary>
        /// Days
        /// </summary>
        public string Days { get; set; }

        /// <summary>
        /// Deletion Status (0: Not Deleted, 1: Deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Update Time
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }
}