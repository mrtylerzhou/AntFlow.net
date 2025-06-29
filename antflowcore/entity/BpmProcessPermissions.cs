namespace antflowcore.entity;

using System;

/// <summary>
/// Represents BPM process permissions.
/// </summary>
public class BpmProcessPermissions
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// User ID.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Department ID.
    /// </summary>
    public long DepId { get; set; }

    /// <summary>
    /// Permission type:
    /// 1: View
    /// 2: Create
    /// 3: Monitor
    /// </summary>
    public int PermissionsType { get; set; }

    /// <summary>
    /// Create user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Create time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// Office ID.
    /// </summary>
    public long OfficeId { get; set; }
}