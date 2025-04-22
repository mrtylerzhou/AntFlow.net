using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the variable sign-up entity.
    /// </summary>
    [Table(Name = "t_bpm_variable_sign_up")]
    public class BpmVariableSignUp
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Variable ID.
        /// </summary>
        [Column(Name = "variable_id")]
        public long VariableId { get; set; }

        /// <summary>
        /// Element ID.
        /// </summary>
        [Column(Name = "element_id")]
        public string ElementId { get; set; }

        /// <summary>
        /// Node ID.
        /// </summary>
        [Column(Name = "node_id")]
        public string NodeId { get; set; }

        /// <summary>
        /// After sign-up way (1: comeback to sign-up user; 2: not callback to sign-up user).
        /// </summary>
        [Column(Name = "after_sign_up_way")]
        public int? AfterSignUpWay { get; set; }

        /// <summary>
        /// Sign-up elements as JSON.
        /// </summary>
        [Column(Name = "sub_elements")]
        public string SubElements { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for delete.
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}