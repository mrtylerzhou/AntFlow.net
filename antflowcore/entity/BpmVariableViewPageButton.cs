using FreeSql;
using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the variable view page button entity.
    /// </summary>
    [Table(Name = "t_bpm_variable_view_page_button")]
    public class BpmVariableViewPageButton
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Variable ID.
        /// </summary>
        [Column(Name = "variable_id")]
        public long VariableId { get; set; }

        /// <summary>
        /// View type: 1 for start user, 2 for other approvals.
        /// </summary>
        [Column(Name = "view_type")]
        public int ViewType { get; set; }

        /// <summary>
        /// Button type: 1-submit, 2-resubmit, 3-agree, 4-disagree, 5-back to modify, 
        /// 6-back to previous node, 7-invalid, 8-print, 9-forward.
        /// </summary>
        [Column(Name = "button_type")]
        public int? ButtonType { get; set; }

        /// <summary>
        /// Button name.
        /// </summary>
        [Column(Name = "button_name")]
        public string ButtonName { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 0 for normal, 1 for delete.
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Update user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}
