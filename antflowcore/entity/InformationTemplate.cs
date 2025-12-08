using System;
using AntFlowCore.Constants;
using AntFlowCore.Enums;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Information template entity.
    /// </summary>
    public class InformationTemplate
    {
        /// <summary>
        /// Auto-incrementing ID.
        /// </summary>
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
        public string SystemTitle { get; set; }

        /// <summary>
        /// System content.
        /// </summary>
        public string SystemContent { get; set; }

        /// <summary>
        /// Email title.
        /// </summary>
        public string MailTitle { get; set; }

        /// <summary>
        /// Email content.
        /// </summary>
        public string MailContent { get; set; }

        /// <summary>
        /// SMS content.
        /// </summary>
        public string NoteContent { get; set; }

        /// <summary>
        /// Jump URL type (1: process approval page, 2: process detail page, 3: todolist page).
        /// </summary>
        public int JumpUrl { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

        /// <summary>
        /// Status (0: enabled, 1: disabled).
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// <see cref="EventTypeEnum"/>
        /// </summary>
        public int? Evt { get; set; }

        public string EventName { get; set; }

        /// <summary>
        /// Deletion status (0: not deleted, 1: deleted).
        /// </summary>
        public int IsDel { get; set; }

        public string TenantId { get; set; }
        /// <summary>
        /// Creation time.
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// User who created this record.
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who last updated this record.
        /// </summary>
        public string UpdateUser { get; set; }
    }
}
