
using FreeSql.DataAnnotations;
using System;

namespace antflowcore.entity
{
    [Table(Name = "bpm_business_process")]
    public class BpmBusinessProcess
    {
        public static readonly int VERSION_DEFAULT_0 = 0;
        public static readonly int VERSION_1 = 1;

        /// <summary>
        /// Process ID (Primary Key)
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process key
        /// </summary>
        [Column(Name = "PROCESSINESS_KEY")]
        public string ProcessinessKey { get; set; }

        /// <summary>
        /// Business ID
        /// </summary>
        [Column(Name = "BUSINESS_ID")]
        public string BusinessId { get; set; }

        /// <summary>
        /// Business Number
        /// </summary>
        [Column(Name = "BUSINESS_NUMBER")]
        public string BusinessNumber { get; set; }

        /// <summary>
        /// Entry ID
        /// </summary>
        [Column(Name = "ENTRY_ID")]
        public string EntryId { get; set; }

        /// <summary>
        /// Process version
        /// </summary>
        [Column(Name = "VERSION")]
        public string Version { get; set; }

        /// <summary>
        /// Create time
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update time
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Process description
        /// </summary>
        [Column(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Process state: 1=approved, 2=approving, 3=canceled
        /// </summary>
        [Column(Name = "process_state")]
        public int ProcessState { get; set; }

        /// <summary>
        /// Created by user
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [Column(Name = "user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// Process digest
        /// </summary>
        [Column(Name = "process_digest")]
        public string ProcessDigest { get; set; }

        /// <summary>
        /// Is deleted: 0=no, 1=yes
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Data source ID (no meaning at the moment)
        /// </summary>
        [Column(Name = "data_source_id")]
        public long? DataSourceId { get; set; }

        /// <summary>
        /// Process instance ID (important for linking business and Activiti processes)
        /// </summary>
        [Column(Name = "PROC_INST_ID_")]
        public string ProcInstId { get; set; }

        /// <summary>
        /// Back to user ID
        /// </summary>
        [Column(Name = "back_user_id")]
        public string BackUserId { get; set; }

        /// <summary>
        /// Is it an external process: 0=no, 1=yes
        /// </summary>
        [Column(Name = "is_out_side_process")]
        public int IsOutSideProcess { get; set; }

        /// <summary>
        /// Is it a low-code flow: 0=no, 1=yes
        /// </summary>
        [Column(Name = "is_lowcode_flow")]
        public int IsLowCodeFlow { get; set; }
    }
}
