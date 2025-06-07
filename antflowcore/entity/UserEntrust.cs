using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a user entrust record.
    /// </summary>
    [Table(Name = "t_user_entrust")]
    public class UserEntrust
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Sender of the entrust
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Receiver ID
        /// </summary>
        [Column(Name = "receiver_id")]
        public string ReceiverId { get; set; }

        /// <summary>
        /// Receiver name
        /// </summary>
        [Column(Name = "receiver_name")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// Power ID
        /// </summary>
        [Column(Name = "power_id")]
        public string PowerId { get; set; }

        /// <summary>
        /// Begin time of the entrust
        /// </summary>
        [Column(Name = "begin_time")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// End time of the entrust
        /// </summary>
        [Column(Name = "end_time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// User who created the record
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// User who updated the record
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        // Default constructor for FreeSQL
        public UserEntrust() { }
    }
}
