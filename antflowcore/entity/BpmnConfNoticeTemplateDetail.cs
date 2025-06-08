using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPMN configuration notice template detail entity.
    /// </summary>
    [Table(Name = "t_bpmn_conf_notice_template_detail")]
    public class BpmnConfNoticeTemplateDetail
    {
        /// <summary>
        /// Primary key ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// BPMN Code.
        /// </summary>
        [Column(Name = "bpmn_code")]
        public string BpmnCode { get; set; }

        /// <summary>
        /// Notice template type.
        /// </summary>
        [Column(Name = "notice_template_type")]
        public int NoticeTemplateType { get; set; }

        /// <summary>
        /// Notice template detail.
        /// </summary>
        [Column(Name = "notice_template_detail")]
        public string NoticeTemplateDetail { get; set; }

        /// <summary>
        /// Is deleted (0 for no, 1 for yes).
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