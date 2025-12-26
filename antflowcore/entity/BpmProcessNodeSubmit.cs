namespace AntFlowCore.Entity;

public class BpmProcessNodeSubmit
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
    /// Back type:
    /// 1: Back to previous node and commit to next node
    /// 2: Back to initiator and commit to next node
    /// 3: Back to initiator and commit to back node
    /// 4: Back to history node and commit to next node
    /// 5: Back to history node and commit to back node
    /// </summary>
    public int BackType { get; set; }

    /// <summary>
    /// Node key.
    /// </summary>
    public string NodeKey { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Creation user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// State.
    /// </summary>
    public int State { get; set; }
    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
}