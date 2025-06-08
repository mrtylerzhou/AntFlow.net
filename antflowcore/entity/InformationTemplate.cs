using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Information template entity.
    /// </summary>
    [Table(Name = "t_information_template")]
    public class InformationTemplate
    {
        /// <summary>
        /// Auto-incrementing ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number.
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// System title.
        /// </summary>
        [Column(Name = "system_title")]
        public string SystemTitle { get; set; }

        /// <summary>
        /// System content.
        /// </summary>
        [Column(Name = "system_content")]
        public string SystemContent { get; set; }

        /// <summary>
        /// Email title.
        /// </summary>
        [Column(Name = "mail_title")]
        public string MailTitle { get; set; }

        /// <summary>
        /// Email content.
        /// </summary>
        [Column(Name = "mail_content")]
        public string MailContent { get; set; }

        /// <summary>
        /// SMS content.
        /// </summary>
        [Column(Name = "note_content")]
        public string NoteContent { get; set; }

        /// <summary>
        /// Jump URL type (1: process approval page, 2: process detail page, 3: todolist page).
        /// </summary>
        [Column(Name = "jump_url")]
        public int JumpUrl { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Status (0: enabled, 1: disabled).
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Deletion status (0: not deleted, 1: deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// User who created this record.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who last updated this record.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }
}
