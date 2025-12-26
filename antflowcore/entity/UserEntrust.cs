using System;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a user entrust record.
    /// </summary>
    public class UserEntrust
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Sender of the entrust
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Receiver ID
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Receiver name
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Power ID
        /// </summary>
        public string PowerId { get; set; }

        /// <summary>
        /// Begin time of the entrust
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// End time of the entrust
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Update time
        /// </summary>
        public DateTime? UpdateTime { get; set; }=DateTime.Now;

        /// <summary>
        /// User who created the record
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// User who updated the record
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// Deletion Status (0: Not Deleted, 1: Deleted)
        /// </summary>
        public int IsDel { get; set; }
        public string TenantId { get; set; }
    }
}