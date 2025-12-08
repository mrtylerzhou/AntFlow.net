using System;
using AntFlowCore.Constants;
using antflowcore.exception;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the external configuration for BPMN.
/// </summary>
public class BpmnOutsideConf
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Form code.
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// Module code.
    /// </summary>
    public string ModuleCode { get; set; }

    /// <summary>
    /// Callback URL.
    /// </summary>
    public string CallBackUrl { get; set; }

    /// <summary>
    /// Detail URL.
    /// </summary>
    public string DetailUrl { get; set; }

    /// <summary>
    /// Deletion flag (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Business name.
    /// </summary>
    public string BusinessName { get; set; }

    /// <summary>
    /// Remarks.
    /// </summary>
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    /// <summary>
    /// Create user ID.
    /// </summary>
    public int CreateUserId { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Modified user ID.
    /// </summary>
    public int ModifiedUserId { get; set; }

    /// <summary>
    /// Modification time.
    /// </summary>
    public DateTime? ModifiedTime { get; set; }

    /// <summary>
    /// Validates the parameters of the configuration.
    /// </summary>
    /// <param name="conf">BpmnOutsideConf instance to validate.</param>
    public static void CheckParams(BpmnOutsideConf conf)
    {
        if (string.IsNullOrEmpty(conf.FormCode)) throw new AFBizException("formCode不能为空!");
        if (string.IsNullOrEmpty(conf.CallBackUrl)) throw new AFBizException("callBackUrl不能为空!");
        if (string.IsNullOrEmpty(conf.DetailUrl)) throw new AFBizException("detailUrl不能为空!");
        if (string.IsNullOrEmpty(conf.BusinessName)) throw new AFBizException("businessName不能为空!");
    }
}
