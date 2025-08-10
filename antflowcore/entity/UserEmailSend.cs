using System;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a user email send record.
    /// </summary>
    public class UserEmailSend
    {
        public int Id { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }
        /// <summary>
        /// Deletion Status (0: Not Deleted, 1: Deleted)
        /// </summary>
        public int IsDel { get; set; }
        public int? TenantId { get; set; }
    }
}