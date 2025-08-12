using System;

namespace AntFlowCore.Entity;

public class BpmFlowruninfo
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process instance ID.
    /// </summary>
    public long RunInfoId { get; set; }

    /// <summary>
    /// Create user ID.
    /// </summary>
    public string CreateUserId { get; set; }

    /// <summary>
    /// Entity key.
    /// </summary>
    public string EntityKey { get; set; }

    /// <summary>
    /// Entity class.
    /// </summary>
    public string EntityClass { get; set; }

    /// <summary>
    /// Entity key type.
    /// </summary>
    public string EntityKeyType { get; set; }

    /// <summary>
    /// Created by.
    /// </summary>
    public string CreateActor { get; set; }

    /// <summary>
    /// Creator department.
    /// </summary>
    public string CreateDepart { get; set; }

    /// <summary>
    /// Creation date.
    /// </summary>
    public DateTime? CreateDate { get; set; }
    /// <summary>
    /// Logical delete flag (0: not deleted, 1: deleted).
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}