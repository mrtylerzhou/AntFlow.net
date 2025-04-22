using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the BPM process application.
    /// </summary>
    public class BpmProcessAppApplication
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Business code (default is empty for the main program).
        /// </summary>
        [Column(Name = "business_code")]
        public string BusinessCode { get; set; }

        /// <summary>
        /// Application name.
        /// </summary>
        [Column(Name = "process_name")]
        public string Title { get; set; }

        /// <summary>
        /// Application type (1: process, 2: app, 3: parent app).
        /// </summary>
        [Column(Name = "apply_type")]
        public int ApplyType { get; set; }

        /// <summary>
        /// Icon for PC.
        /// </summary>
        [Column(Name = "pc_icon")]
        public string PcIcon { get; set; }

        /// <summary>
        /// Icon for mobile platform.
        /// </summary>
        [Column(Name = "effective_source")]
        public string EffectiveSource { get; set; }

        /// <summary>
        /// Is child app (0: no, 1: yes).
        /// </summary>
        [Column(Name = "is_son")]
        public int IsSon { get; set; }

        /// <summary>
        /// View URL.
        /// </summary>
        [Column(Name = "look_url")]
        public string LookUrl { get; set; }

        /// <summary>
        /// Submit URL.
        /// </summary>
        [Column(Name = "submit_url")]
        public string SubmitUrl { get; set; }

        /// <summary>
        /// Condition URL.
        /// </summary>
        [Column(Name = "condition_url")]
        public string ConditionUrl { get; set; }

        /// <summary>
        /// Parent app ID.
        /// </summary>
        [Column(Name = "parent_id")]
        public int ParentId { get; set; }

        /// <summary>
        /// Application URL.
        /// </summary>
        [Column(Name = "application_url")]
        public string ApplicationUrl { get; set; }

        /// <summary>
        /// App route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Permission code.
        /// </summary>
        [Column(Name = "permissions_code")]
        public string PermissionsCode { get; set; }

        /// <summary>
        /// Deletion status: 0 for not deleted, 1 for deleted.
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creator's user ID.
        /// </summary>
        [Column(Name = "create_user_id")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Is for all (page with no configuration function, default writes to database).
        /// </summary>
        [Column(Name = "is_all")]
        public int IsAll { get; set; }

        /// <summary>
        /// Process state: 0 for no, 1 for yes.
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Sort order.
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// User request URI.
        /// </summary>
        [Column(Name = "user_request_uri")]
        public string UserRequestUri { get; set; }

        /// <summary>
        /// Role request URI.
        /// </summary>
        [Column(Name = "role_request_uri")]
        public string RoleRequestUri { get; set; }
    }
}