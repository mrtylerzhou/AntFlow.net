using AntFlowCore.Constants;

namespace antflowcore.entity;

using System;

/// <summary>
/// Represents a message for a BPM variable.
/// </summary>
public class BpmVariableMessage
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
    /// Message type (1: out of node message, 2: node inner message).
    /// </summary>
    public int MessageType { get; set; }

    /// <summary>
    /// Event type.
    /// </summary>
    public int EventType { get; set; }

    /// <summary>
    /// Notice content.
    /// </summary>
    public string Content { get; set; }

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