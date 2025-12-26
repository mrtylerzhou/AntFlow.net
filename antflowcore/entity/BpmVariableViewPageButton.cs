using System;
using AntFlowCore.Constants;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the variable view page button entity.
    /// </summary>
    public class BpmVariableViewPageButton
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Variable ID.
        /// </summary>
        public long VariableId { get; set; }

        /// <summary>
        /// View type: 1 for start user, 2 for other approvals.
        /// </summary>
        public int ViewType { get; set; }

        /// <summary>
        /// Button type: 1-submit, 2-resubmit, 3-agree, 4-disagree, 5-back to modify, 
        /// 6-back to previous node, 7-invalid, 8-print, 9-forward.
        /// </summary>
        public int? ButtonType { get; set; }

        /// <summary>
        /// Button name.
        /// </summary>
        public string ButtonName { get; set; }

        /// <summary>
        /// Remark.
        /// </summary>
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

        /// <summary>
        /// 0 for normal, 1 for delete.
        /// </summary>
        public int IsDel { get; set; }
        public string TenantId { get; set; }

        /// <summary>
        /// Create user.
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// Create time.
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Update user.
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        public DateTime? UpdateTime { get; set; }=DateTime.Now;
    }
}