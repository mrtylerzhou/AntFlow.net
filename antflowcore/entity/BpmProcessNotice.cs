namespace AntFlowCore.Entity;

public class BpmProcessNotice
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Notice type:
    /// 1: Mail
    /// 2: SMS
    /// 3: App
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcessKey { get; set; }
    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}