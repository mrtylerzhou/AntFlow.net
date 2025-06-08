using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// BPMN Node Button Configuration
    /// </summary>
    [Table(Name = "t_bpmn_node_button_conf")]
    public class BpmnNodeButtonConf
    {
        /// <summary>
        /// ID
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// BPMN Node ID
        /// </summary>
        [Column(Name = "bpmn_node_id")]
        public long BpmnNodeId { get; set; }

        /// <summary>
        /// Button Page Type (1: initiate page, 2: approval page)
        /// </summary>
        [Column(Name = "button_page_type")]
        public int ButtonPageType { get; set; }

        /// <summary>
        /// Button Type (e.g., 1: submit, 2: resubmit, etc.)
        /// </summary>
        [Column(Name = "button_type")]
        public int ButtonType { get; set; }

        /// <summary>
        /// Button Name
        /// </summary>
        [Column(Name = "button_name")]
        public string ButtonName { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// Deletion Status (0: not deleted, 1: deleted)
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update Time
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}
