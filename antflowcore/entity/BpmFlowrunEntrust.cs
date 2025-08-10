using System;

namespace AntFlowCore.Entity;

public class BpmFlowrunEntrust
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Process instance ID (not the task ID).
    /// </summary>
    public string RunInfoId { get; set; }

    /// <summary>
    /// Current running task ID.
    /// </summary>
    public string RunTaskId { get; set; }

    /// <summary>
    /// Original assignee.
    /// </summary>
    public string Original { get; set; }

    /// <summary>
    /// Original assignee name.
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// Actual assignee.
    /// </summary>
    public string Actual { get; set; }

    /// <summary>
    /// Actual assignee name.
    /// </summary>
    public string ActualName { get; set; }

    /// <summary>
    /// Type of task (1 for entrust task, 2 for circulate task).
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Whether the task is read (0 for no, 1 for yes).
    /// </summary>
    public int IsRead { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    /// Whether the task is viewed (0 for no, 1 for yes).
    /// </summary>
    public int IsView { get; set; }
    /// <summary>
    /// Logical delete flag (0: not deleted, 1: deleted).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
}