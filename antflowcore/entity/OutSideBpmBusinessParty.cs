using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents an external BPM business party.
    /// </summary>
    [Table(Name = "t_out_side_bpm_business_party")]
    public class OutSideBpmBusinessParty
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Business party mark.
        /// </summary>
        [Column(Name = "business_party_mark")]
        public string BusinessPartyMark { get; set; }

        /// <summary>
        /// Business party name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Business type: 0 for embedded, 1 for API access.
        /// </summary>
        [Column(Name = "type")]
        public int Type { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion status: 0 for normal, 1 for deleted.
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Creator of the record.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last updater of the record.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time", IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}