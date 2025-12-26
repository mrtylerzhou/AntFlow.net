namespace AntFlowCore.Entity;

public class BpmProcessNodeRecord
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process instance ID.
    /// </summary>
    public string ProcessInstanceId { get; set; }

    /// <summary>
    /// Task ID.
    /// </summary>
    public string TaskId { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;
    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}