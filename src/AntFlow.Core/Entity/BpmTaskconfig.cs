namespace AntFlow.Core.Entity;

/// <summary>
///     Represents task configuration.
/// </summary>
public class BpmTaskconfig
{
    /// <summary>
    ///     Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Process definition ID.
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     Task definition key.
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    ///     User ID.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    ///     User number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    ///     Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }
}