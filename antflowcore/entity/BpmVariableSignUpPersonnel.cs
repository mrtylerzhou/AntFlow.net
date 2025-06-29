namespace antflowcore.entity;

using System;

/// <summary>
/// Represents the variable sign-up personnel entity.
/// </summary>
public class BpmVariableSignUpPersonnel
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Variable ID.
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    /// Element ID.
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// Assignee.
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// Assignee Name.
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    /// Remark.
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 0 for normal, 1 for delete.
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Create user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Create time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}