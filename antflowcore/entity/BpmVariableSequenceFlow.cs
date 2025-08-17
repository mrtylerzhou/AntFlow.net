namespace antflowcore.entity;

using System;

/// <summary>
/// Represents a sequence flow for a BPM variable.
/// </summary>
public class BpmVariableSequenceFlow
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
    /// Element name.
    /// </summary>
    public string ElementName { get; set; }

    /// <summary>
    /// Element start flow ID.
    /// </summary>
    public string ElementFromId { get; set; }

    /// <summary>
    /// Element to ID (connected node).
    /// </summary>
    public string ElementToId { get; set; }

    /// <summary>
    /// Flow type (1: no param, 2: has param).
    /// </summary>
    public int SequenceFlowType { get; set; }

    /// <summary>
    /// Flow conditions.
    /// </summary>
    public string SequenceFlowConditions { get; set; } = "";

    /// <summary>
    /// Remark.
    /// </summary>
    public string Remark { get; set; } = "";

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