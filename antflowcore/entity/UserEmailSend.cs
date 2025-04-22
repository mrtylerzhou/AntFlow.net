using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a user email send record.
    /// </summary>
    [Table(Name = "t_user_email_send")]
    public class UserEmailSend
    {
        private static readonly long serialVersionUID = 1L;

        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Sender's email address
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Receiver's email address
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// Email title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Email content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Create user
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Update user
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        // Default constructor for FreeSQL
        public UserEmailSend()
        { }
    }
}