using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the verify information entity.
    /// </summary>
    [Table(Name = "bpm_verify_info")]
    public class BpmVerifyInfo
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        [Column(Name = "run_info_id")]
        public string RunInfoId { get; set; }

        /// <summary>
        /// Verify user ID.
        /// </summary>
        [Column(Name = "verify_user_id")]
        public string VerifyUserId { get; set; }

        /// <summary>
        /// Verify user name.
        /// </summary>
        [Column(Name = "verify_user_name")]
        public string VerifyUserName { get; set; }

        /// <summary>
        /// Verify status: 1-submit, 2-agree, 3-not agree.
        /// </summary>
        [Column(Name = "verify_status")]
        public int VerifyStatus { get; set; }

        /// <summary>
        /// Verify description.
        /// </summary>
        [Column(Name = "verify_desc")]
        public string VerifyDesc { get; set; }

        /// <summary>
        /// Verify date.
        /// </summary>
        [Column(Name = "verify_date")]
        public DateTime? VerifyDate { get; set; }

        /// <summary>
        /// Task name.
        /// </summary>
        [Column(Name = "task_name")]
        public string TaskName { get; set; }

        /// <summary>
        /// Task ID.
        /// </summary>
        [Column(Name = "task_id")]
        public string TaskId { get; set; }
        [Column(Name = "task_def_key")]
        public String TaskDefKey { get; set; }

        /// <summary>
        /// Business type.
        /// </summary>
        [Column(Name = "business_type")]
        public int BusinessType { get; set; }

        /// <summary>
        /// Business ID.
        /// </summary>
        [Column(Name = "business_id")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Original assignee ID.
        /// </summary>
        [Column(Name = "original_id")]
        public string OriginalId { get; set; }

        /// <summary>
        /// Process code.
        /// </summary>
        [Column(Name = "process_code")]
        public string ProcessCode { get; set; }
    }
}
