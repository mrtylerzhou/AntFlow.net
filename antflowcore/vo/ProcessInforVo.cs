using System;
using antflowcore.entity;

namespace AntFlowCore.Vo
{
    public class ProcessInforVo
    {
        /// <summary>
        /// bpm business process connection
        /// </summary>
        public BpmBusinessProcess BpmBusinessProcess { get; set; }

        /// <summary>
        /// processiness key
        /// </summary>
        public string ProcessinessKey { get; set; }

        /// <summary>
        /// business number
        /// </summary>
        public string BusinessNumber { get; set; }

        /// <summary>
        /// receiver Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// process type
        /// </summary>
        public string ProcessType { get; set; }

        /// <summary>
        /// process name
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// others id
        /// </summary>
        public string OtherUserId { get; set; }

        /// <summary>
        /// carbon copy
        /// </summary>
        public string[] Cc { get; set; }

        /// <summary>
        /// email url
        /// </summary>
        public string EmailUrl { get; set; }

        /// <summary>
        /// in site message url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// App push url
        /// </summary>
        public string AppPushUrl { get; set; }

        /// <summary>
        /// task id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// process operation type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// node id
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// form code
        /// </summary>
        public string FormCode { get; set; }
    }
}