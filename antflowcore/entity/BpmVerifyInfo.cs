using System;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the verify information entity.
    /// </summary>
    public class BpmVerifyInfo
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        public string RunInfoId { get; set; }

        /// <summary>
        /// Verify user ID.
        /// </summary>
        public string VerifyUserId { get; set; }

        /// <summary>
        /// Verify user name.
        /// </summary>
        public string VerifyUserName { get; set; }

        /// <summary>
        /// Verify status: 1-submit, 2-agree, 3-not agree.
        /// </summary>
        public int VerifyStatus { get; set; }

        /// <summary>
        /// Verify description.
        /// </summary>
        public string VerifyDesc { get; set; }

        /// <summary>
        /// Verify date.
        /// </summary>
        public DateTime? VerifyDate { get; set; }

        /// <summary>
        /// Task name.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Task ID.
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Task definition key.
        /// </summary>
        public string TaskDefKey { get; set; }

        /// <summary>
        /// Business type.
        /// </summary>
        public int BusinessType { get; set; }

        /// <summary>
        /// Business ID.
        /// </summary>
        public string BusinessId { get; set; }

        /// <summary>
        /// Original assignee ID.
        /// </summary>
        public string OriginalId { get; set; }

        /// <summary>
        /// Process code.
        /// </summary>
        public string ProcessCode { get; set; }
    }
}