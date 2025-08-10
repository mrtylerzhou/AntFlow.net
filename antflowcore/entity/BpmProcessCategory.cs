using System;

namespace AntFlowCore.Entity;


/// <summary>
/// Represents the process category.
/// </summary>
public class BpmProcessCategory
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process type name.
    /// </summary>
    public string ProcessTypeName { get; set; }

    /// <summary>
    /// Deletion state (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
    /// <summary>
    /// State of the category.
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// Indicates if it is for the app (0 for no, 1 for yes).
    /// </summary>
    public int IsApp { get; set; }

    /// <summary>
    /// Entrance URL.
    /// </summary>
    public string Entrance { get; set; }
}