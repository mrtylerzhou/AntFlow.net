using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// BPMN Configuration Notice Template
    /// </summary>
    [Table(Name = "t_bpmn_conf_notice_template")]
    public class BpmnConfNoticeTemplate
    {
        /// <summary>
        /// ID (Auto Increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// BPMN Code
        /// </summary>
        [Column(Name = "bpmn_code")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// Deletion Status (0: Normal, 1: Deleted)
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