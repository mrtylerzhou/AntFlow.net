using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the process application type.
/// </summary>
public class BpmProcessApplicationType
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Application ID.
    /// </summary>
    public long ApplicationId { get; set; }

    /// <summary>
    /// Category ID.
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// Deletion state (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// Frequently used state (0 for no, 1 for yes).
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// History ID.
    /// </summary>
    public long HistoryId { get; set; }

    /// <summary>
    /// Visibility state (0 for hidden, 1 for visible).
    /// </summary>
    public int VisbleState { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Common use state.
    /// </summary>
    public int CommonUseState { get; set; }
}