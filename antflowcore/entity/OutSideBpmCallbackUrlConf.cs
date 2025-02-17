using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for external BPM business party callback URLs.
    /// </summary>
    [Table(Name = "t_out_side_bpm_callback_url_conf")]
    public class OutSideBpmCallbackUrlConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Business party ID.
        /// </summary>
        [Column(Name = "business_party_id")]
        public long BusinessPartyId { get; set; }

        /// <summary>
        /// BPMN configuration ID.
        /// </summary>
        [Column(Name = "bpmn_conf_id")]
        public long BpmnConfId { get; set; }

        /// <summary>
        /// Form code.
        /// </summary>
        [Column(Name = "form_code")]
        public string FormCode { get; set; }

        /// <summary>
        /// BPMN configuration callback URL.
        /// </summary>
        [Column(Name = "bpm_conf_callback_url")]
        public string BpmConfCallbackUrl { get; set; }

        /// <summary>
        /// Process flow callback URL.
        /// </summary>
        [Column(Name = "bpm_flow_callback_url")]
        public string BpmFlowCallbackUrl { get; set; }

        /// <summary>
        /// API client ID.
        /// </summary>
        [Column(Name = "api_client_id")]
        public string ApiClientId { get; set; }

        /// <summary>
        /// API client secret.
        /// </summary>
        [Column(Name = "api_client_secret")]
        public string ApiClientSecret { get; set; }

        /// <summary>
        /// Status: 0 for enabled, 1 for disabled.
        /// </summary>
        [Column(Name = "status")]
        public int Status { get; set; }

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
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last updater of the record.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}
