namespace antflowcore.entity;

using System;

/// <summary>
/// Represents process variables.
/// </summary>
public class BpmVariable
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process number.
    /// </summary>
    public string ProcessNum { get; set; }

    /// <summary>
    /// Process name.
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// Process description.
    /// </summary>
    public string ProcessDesc { get; set; } = "";

    /// <summary>
    /// Process start conditions.
    /// </summary>
    public string ProcessStartConditions { get; set; }

    /// <summary>
    /// BPMN code.
    /// </summary>
    public string BpmnCode { get; set; }

    /// <summary>
    /// Remark.
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    /// Indicates whether the record is deleted (0 normal, 1 deleted).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Create user.
    /// </summary>
    public string? CreateUser { get; set; }

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