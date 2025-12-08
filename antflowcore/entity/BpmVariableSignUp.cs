using AntFlowCore.Constants;

namespace antflowcore.entity;

using System;

/// <summary>
/// Represents the variable sign-up entity.
/// </summary>
public class BpmVariableSignUp
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
    /// Element ID.
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// Node ID.
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    /// After sign-up way (1: comeback to sign-up user; 2: not callback to sign-up user).
    /// </summary>
    public int? AfterSignUpWay { get; set; }

    /// <summary>
    /// Sign-up elements as JSON.
    /// </summary>
    public string SubElements { get; set; }

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
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}