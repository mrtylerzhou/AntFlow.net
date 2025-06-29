using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the operation log entity.
    /// </summary>
    [Table(Name = "t_op_log")]
    public class OpLog
    {

        /// <summary>
        /// Primary key
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Message ID
        /// </summary>
        [Column(Name = "msg_id")]
        public string MsgId { get; set; }

        /// <summary>
        /// Operation flag: 0=success, 1=failure, 2=business exception
        /// </summary>
        [Column(Name = "op_flag")]
        public int OpFlag { get; set; }

        /// <summary>
        /// Operator account number
        /// </summary>
        [Column(Name = "op_user_no")]
        public string OpUserNo { get; set; }

        /// <summary>
        /// Operator account name
        /// </summary>
        [Column(Name = "op_user_name")]
        public string OpUserName { get; set; }

        /// <summary>
        /// Operation method
        /// </summary>
        [Column(Name = "op_method")]
        public string OpMethod { get; set; }

        /// <summary>
        /// Operation time
        /// </summary>
        [Column(Name = "op_time")]
        public DateTime? OpTime { get; set; }

        /// <summary>
        /// Operation duration (in milliseconds)
        /// </summary>
        [Column(Name = "op_use_time")]
        public long OpUseTime { get; set; }

        /// <summary>
        /// Operation parameters
        /// </summary>
        [Column(Name = "op_param")]
        public string OpParam { get; set; }

        /// <summary>
        /// Operation result
        /// </summary>
        [Column(Name = "op_result")]
        public string OpResult { get; set; }

        /// <summary>
        /// System type (iOS, Android, 1=PC)
        /// </summary>
        [Column(Name = "system_type")]
        public string SystemType { get; set; }

        /// <summary>
        /// App version
        /// </summary>
        [Column(Name = "app_version")]
        public string AppVersion { get; set; }

        /// <summary>
        /// Device type
        /// </summary>
        [Column(Name = "hardware")]
        public string Hardware { get; set; }

        /// <summary>
        /// System version
        /// </summary>
        [Column(Name = "system_version")]
        public string SystemVersion { get; set; }

        /// <summary>
        /// Operation remarks
        /// </summary>
        public string Remark { get; set; }

        // Default constructor for FreeSQL
        public OpLog() { }
    }
}
