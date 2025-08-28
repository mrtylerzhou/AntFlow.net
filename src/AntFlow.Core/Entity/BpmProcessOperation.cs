namespace AntFlow.Core.Entity;

public class BpmProcessOperation
{
    public long Id { get; set; }
    public string? ProcessKey { get; set; }
    public string? ProcessNode { get; set; }

    /// <summary>
    ///     1: batch submit, 2: entrust
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    ///     Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }
}