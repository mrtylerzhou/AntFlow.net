using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the BPMN node "to" configuration.
/// </summary>
public class BpmnNodeTo
{
    /// <summary>
    /// Auto-incrementing ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// BPMN node ID.
    /// </summary>
    public long BpmnNodeId { get; set; }

    /// <summary>
    /// Node "to" value.
    /// </summary>
    public string NodeTo { get; set; }

    /// <summary>
    /// Remark or additional information.
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// Deletion status (0 for normal, 1 for delete).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// User who created this record.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Time when this record was created.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// User who last updated this record.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Time when this record was last updated.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}