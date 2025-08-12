namespace AntFlowCore.Entity;

public class BpmProcessNodeOvertime
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Notice type (1: email, 2: SMS, 3: app).
    /// </summary>
    public int NoticeType { get; set; }

    /// <summary>
    /// Node name.
    /// </summary>
    public string NodeName { get; set; }

    /// <summary>
    /// Node key.
    /// </summary>
    public string NodeKey { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// Notice time.
    /// </summary>
    public int NoticeTime { get; set; }
    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}