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
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who created the record
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// User who updated the record
        /// </summary>
        public string UpdateUser { get; set; }
    }
}