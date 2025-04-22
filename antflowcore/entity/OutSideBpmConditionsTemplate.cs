using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the conditions template for the external BPM system.
    /// </summary>
    [Table(Name = "t_out_side_bpm_conditions_template")]
    public class OutSideBpmConditionsTemplate
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Business party ID
        /// </summary>
        [Column(Name = "business_party_id")]
        public long BusinessPartyId { get; set; }

        /// <summary>
        /// Template mark
        /// </summary>
        [Column(Name = "template_mark")]
        public string TemplateMark { get; set; }

        /// <summary>
        /// Template name
        /// </summary>
        [Column(Name = "template_name")]
        public string TemplateName { get; set; }

        /// <summary>
        /// Application ID
        /// </summary>
        [Column(Name = "application_id")]
        public int ApplicationId { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for deleted
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creator username
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updater username
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Creator user ID
        /// </summary>
        [Column(Name = "create_user_id")]
        public string CreateUserId { get; set; }

        // Default constructor for FreeSQL
        public OutSideBpmConditionsTemplate()
        { }
    }
}