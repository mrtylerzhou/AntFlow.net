using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the third party process service, business party admin person.
    /// </summary>
    [Table(Name = "t_out_side_bpm_admin_personnel")]
    public class OutSideBpmAdminPersonnel
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
        /// Administrator type (1-process administrator, 2-application administrator, 3-interface administrator)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Administrator ID
        /// </summary>
        [Column(Name = "employee_id")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// Administrator's name
        /// </summary>
        [Column(Name = "employee_name")]
        public string EmployeeName { get; set; }

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

        // Default constructor for FreeSQL
        public OutSideBpmAdminPersonnel()
        { }
    }
}