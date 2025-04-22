using antflowcore.exception;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the external configuration for BPMN.
    /// </summary>
    [Table(Name = "t_bpmn_outside_conf")]
    public class BpmnOutsideConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Form code.
        /// </summary>
        [Column(Name = "form_code")]
        public string FormCode { get; set; }

        /// <summary>
        /// Module code.
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// Callback URL.
        /// </summary>
        [Column(Name = "call_back_url")]
        public string CallBackUrl { get; set; }

        /// <summary>
        /// Detail URL.
        /// </summary>
        [Column(Name = "detail_url")]
        public string DetailUrl { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Business name.
        /// </summary>
        [Column(Name = "business_name")]
        public string BusinessName { get; set; }

        /// <summary>
        /// Remarks.
        /// </summary>
        [Column(Name = "remark")]
        public string Remark { get; set; }

        /// <summary>
        /// Create user ID.
        /// </summary>
        [Column(Name = "create_user_id")]
        public int CreateUserId { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time", IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Modified user ID.
        /// </summary>
        [Column(Name = "modified_user_id")]
        public int ModifiedUserId { get; set; }

        /// <summary>
        /// Modification time.
        /// </summary>
        [Column(Name = "modified_time")]
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// Validates the parameters of the configuration.
        /// </summary>
        /// <param name="conf">BpmnOutsideConf instance to validate.</param>
        public static void CheckParams(BpmnOutsideConf conf)
        {
            if (string.IsNullOrEmpty(conf.FormCode))
            {
                throw new AFBizException("formCode不能为空!");
            }
            if (string.IsNullOrEmpty(conf.CallBackUrl))
            {
                throw new AFBizException("callBackUrl不能为空!");
            }
            if (string.IsNullOrEmpty(conf.DetailUrl))
            {
                throw new AFBizException("detailUrl不能为空!");
            }
            if (string.IsNullOrEmpty(conf.BusinessName))
            {
                throw new AFBizException("businessName不能为空!");
            }
        }
    }
}