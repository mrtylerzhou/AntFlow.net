using AntFlowCore.Base.util;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Base.entity
{
    /// <summary>
    /// Represents the operation log entity.
    /// </summary>
  
    public class OpLog
    {

        /// <summary>
        /// Primary key
        /// </summary>
    
        public long Id { get; set; }

        /// <summary>
        /// Message ID
        /// </summary>
      
        public string MsgId { get; set; }

        /// <summary>
        /// Operation flag: 0=success, 1=failure, 2=business exception
        /// </summary>
      
        public int OpFlag { get; set; }

        /// <summary>
        /// Operator account number
        /// </summary>
    
        public string OpUserNo { get; set; }

        /// <summary>
        /// Operator account name
        /// </summary>
       
        public string OpUserName { get; set; }

        /// <summary>
        /// Operation method
        /// </summary>
       
        public string OpMethod { get; set; }

        /// <summary>
        /// Operation time
        /// </summary>
     
        public DateTime? OpTime { get; set; }

        /// <summary>
        /// Operation duration (in milliseconds)
        /// </summary>
     
        public long OpUseTime { get; set; }

        /// <summary>
        /// Operation parameters
        /// </summary>
      
        public string OpParam { get; set; }

        /// <summary>
        /// Operation result
        /// </summary>
      
        public string OpResult { get; set; }

        /// <summary>
        /// System type (iOS, Android, 1=PC)
        /// </summary>
       
        public string SystemType { get; set; }

        /// <summary>
        /// App version
        /// </summary>
      
        public string AppVersion { get; set; }

        /// <summary>
        /// Device type
        /// </summary>
       
        public string Hardware { get; set; }

        /// <summary>
        /// System version
        /// </summary>
      
        public string SystemVersion { get; set; }

        /// <summary>
        /// Operation remarks
        /// </summary>
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

        // Default constructor for FreeSQL
        public OpLog() { }
    }
}
