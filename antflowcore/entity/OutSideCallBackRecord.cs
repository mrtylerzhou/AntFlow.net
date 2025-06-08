using FreeSql;
using System;
using FreeSql.DataAnnotations; // Assuming enums are defined in this namespace

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents an external callback record.
    /// </summary>
    [Table(Name = "t_out_side_bpm_call_back_record")]
    public class OutSideCallBackRecord
    {
        /// <summary>
        /// Primary key (auto-increment)
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Process number
        /// </summary>
        [Column(Name = "process_number")]
        public string ProcessNumber { get; set; }

        /// <summary>
        /// Push status (0 for success, 1 for fail)
        /// </summary>
        [Column(Name = "status")]
        public int Status { get; set; }

        /// <summary>
        /// Retry times
        /// </summary>
        [Column(Name = "retry_times")]
        public int RetryTimes { get; set; }

        /// <summary>
        /// Operation type (uses MsgProcessEventEnum)
        /// </summary>
        [Column(Name = "button_operation_type")]
        public int ButtonOperationType { get; set; }

        /// <summary>
        /// Callback type name (uses CallbackTypeEnum)
        /// </summary>
        [Column(Name = "call_back_type_name")]
        public string CallBackTypeName { get; set; }

        /// <summary>
        /// Business ID
        /// </summary>
        [Column(Name = "business_id")]
        public long BusinessId { get; set; }

        /// <summary>
        /// Form code
        /// </summary>
        [Column(Name = "form_code")]
        public string FormCode { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted)
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
        public OutSideCallBackRecord() { }
    }
}
