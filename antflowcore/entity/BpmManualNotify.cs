using System;

namespace AntFlowCore.Entity;

public class BpmManualNotify
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Business ID.
    /// </summary>
    public long BusinessId { get; set; }

    /// <summary>
    /// Process code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Last reminder time.
    /// </summary>
    public DateTime? LastTime { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
    /// <summary>
    /// Logical delete flag (0: not deleted, 1: deleted).
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}