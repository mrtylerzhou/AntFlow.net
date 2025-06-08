using System;
using antflowcore.constant.enus;
using antflowcore.util;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a button on the BPMN view page.
    /// </summary>
    [Table(Name = "t_bpmn_view_page_button")]
    public class BpmnViewPageButton
    {
        /// <summary>
        /// Gets or sets the auto-incrementing ID.
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the configuration ID.
        /// </summary>
        [Column(Name = "conf_id")]
        public long ConfId { get; set; }

        /// <summary>
        /// Gets or sets the view type: 1 for start user, 2 for other approver.
        /// </summary>
        [Column(Name = "view_type")]
        public int ViewType { get; set; }

        /// <summary>
        /// Gets or sets the button type: 
        /// 1-submit, 2-reSubmit, 3-agree, 4-disagree, 5-back, 
        /// 6-backToPrevNode, 7-cancel, 8-print, 9-forward.
        /// </summary>
        [Column(Name = "button_type")]
        public int ButtonType { get; set; }

        /// <summary>
        /// Gets or sets the name of the button.
        /// </summary>
        [Column(Name = "button_name")]
        public string ButtonName { get; set; }

        /// <summary>
        /// Gets or sets the remark for the button.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the deletion status (1 for deleted, 0 for not deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Gets or sets the user who created the button.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Gets or sets the time the button was created.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the user who last updated the button.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Gets or sets the time the button was last updated.
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Builds a new instance of <see cref="BpmnViewPageButton"/>.
        /// </summary>
        /// <param name="confId">The configuration ID.</param>
        /// <param name="buttonTypeCode">The button type code.</param>
        /// <param name="viewPageTypeCode">The view page type code.</param>
        /// <returns>A new instance of <see cref="BpmnViewPageButton"/>.</returns>
        public static BpmnViewPageButton BuildViewPageButton(long confId, int buttonTypeCode, int viewPageTypeCode)
        {
            return new BpmnViewPageButton
            {
                ConfId = confId,
                ViewType = viewPageTypeCode,
                ButtonType = buttonTypeCode,
                ButtonName = ButtonTypeEnumExtensions.GetDescByCode(buttonTypeCode),
                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                CreateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpNameSafe(),
                UpdateTime = DateTime.Now
            };
        }
    }
}
