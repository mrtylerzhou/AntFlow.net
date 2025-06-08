using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the third party business access table.
    /// </summary>
    [Table(Name = "t_out_side_bpm_access_business")]
    public class OutSideBpmAccessBusiness
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
        /// BPMN configuration ID
        /// </summary>
        [Column(Name = "bpmn_conf_id")]
        public long BpmnConfId { get; set; }

        /// <summary>
        /// Form code
        /// </summary>
        [Column(Name = "form_code")]
        public string FormCode { get; set; }

        /// <summary>
        /// Process number
        /// </summary>
        [Column(Name = "process_number")]
        public string ProcessNumber { get; set; }

        /// <summary>
        /// Form data on PC
        /// </summary>
        [Column(Name = "form_data_pc")]
        public string FormDataPc { get; set; }

        /// <summary>
        /// Form data on app
        /// </summary>
        [Column(Name = "form_data_app")]
        public string FormDataApp { get; set; }

        /// <summary>
        /// Template mark
        /// </summary>
        [Column(Name = "template_mark")]
        public string TemplateMark { get; set; }

        /// <summary>
        /// Start username
        /// </summary>
        [Column(Name = "start_username")]
        public string StartUsername { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 0 for no, 1 for yes (deleted or not)
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
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updater username
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        // Default constructor for FreeSQL
        public OutSideBpmAccessBusiness() { }
    }
}
