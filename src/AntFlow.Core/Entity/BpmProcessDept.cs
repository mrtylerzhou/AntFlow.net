namespace AntFlow.Core.Entity;

/// <summary>
///     Represents a BPM process department.
/// </summary>
public class BpmProcessDept
{
    /// <summary>
    ///     Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Process code.
    /// </summary>
    public string ProcessCode { get; set; }

    /// <summary>
    ///     Process type.
    /// </summary>
    public int ProcessType { get; set; }

    /// <summary>
    ///     Process name.
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    ///     Department ID to which the process belongs.
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    ///     Process remark.
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    ///     Create time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Create user ID.
    /// </summary>
    public long CreateUser { get; set; }

    /// <summary>
    ///     Update user ID.
    /// </summary>
    public long UpdateUser { get; set; }

    /// <summary>
    ///     Modify time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    ///     Is deleted (0 for no, 1 for yes).
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     Is for all (0 for no, 1 for yes).
    /// </summary>
    public int IsAll { get; set; }
}