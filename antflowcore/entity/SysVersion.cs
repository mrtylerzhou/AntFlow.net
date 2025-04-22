using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the system version configuration.
    /// </summary>
    [Table(Name = "t_sys_version")]
    public class SysVersion
    {
        /// <summary>
        /// Published status (0 for published, 1 for unpublished)
        /// </summary>
        public static readonly int HIDE_STATUS_0 = 0;
        public static readonly int HIDE_STATUS_1 = 1;

        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Version description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Force update (0 for no, 1 for yes)
        /// </summary>
        [Column(Name = "is_force")]
        public int IsForce { get; set; }

        /// <summary>
        /// Android download URL
        /// </summary>
        [Column(Name = "android_url")]
        public string AndroidUrl { get; set; }

        /// <summary>
        /// iOS download URL
        /// </summary>
        [Column(Name = "ios_url")]
        public string IosUrl { get; set; }

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

        /// <summary>
        /// Hide status (0 for not hide, 1 for hide)
        /// </summary>
        [Column(Name = "is_hide")]
        public int IsHide { get; set; }

        /// <summary>
        /// Download code
        /// </summary>
        [Column(Name = "download_code")]
        public string DownloadCode { get; set; }

        /// <summary>
        /// Effective time
        /// </summary>
        [Column(Name = "effective_time")]
        public DateTime? EffectiveTime { get; set; }

        // Default constructor for FreeSQL
        public SysVersion() { }
    }
}
