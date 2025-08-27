namespace AntFlow.Core.Entity;

/// <summary>
///     Process Name Entity
/// </summary>
public class BpmProcessName
{
    /// <summary>
    ///     Auto-increment ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Process Name
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    ///     Deletion Status (0: Normal, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     Creation Time
    /// </summary>
    public DateTime? CreateTime { get; set; }
}