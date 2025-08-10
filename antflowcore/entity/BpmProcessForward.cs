using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents a BPM process forward.
/// </summary>
public class BpmProcessForward
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Forward user ID.
    /// </summary>
    public string ForwardUserId { get; set; }

    /// <summary>
    /// Forward user's name.
    /// </summary>
    public string ForwardUserName { get; set; }

    /// <summary>
    /// Process instance ID.
    /// </summary>
    public string ProcessInstanceId { get; set; }

    /// <summary>
    /// Create time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Create user ID.
    /// </summary>
    public string CreateUserId { get; set; }

    /// <summary>
    /// Is deleted (0 for no, 1 for yes).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }

    /// <summary>
    /// Is read (0 for no, 1 for yes).
    /// </summary>
    public int IsRead { get; set; }

    /// <summary>
    /// Task ID.
    /// </summary>
    public string TaskId { get; set; }

    /// <summary>
    /// Process number.
    /// </summary>
    public string ProcessNumber { get; set; }
    public string NodeId { get; set; }
}