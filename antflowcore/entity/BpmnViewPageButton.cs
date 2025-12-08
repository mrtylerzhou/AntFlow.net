using System;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.util;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents a button on the BPMN view page.
/// </summary>
public class BpmnViewPageButton
{
    public long Id { get; set; }

    public long ConfId { get; set; }

    /// <summary>
    /// View type: 1 for start user, 2 for other approver.
    /// </summary>
    public int ViewType { get; set; }

    /// <summary>
    /// Button type: 1-submit, 2-reSubmit, 3-agree, 4-disagree, 5-back,
    /// 6-backToPrevNode, 7-cancel, 8-print, 9-forward.
    /// </summary>
    public int ButtonType { get; set; }

    public string ButtonName { get; set; }

    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    public int IsDel { get; set; }
    public string TenantId { get; set; }

    public string CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public string UpdateUser { get; set; }

    public DateTime? UpdateTime { get; set; }

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
            UpdateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            Remark = StringConstants.BIG_WHITE_BLANK,
        };
    }
}