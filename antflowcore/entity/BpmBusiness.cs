using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the business entity in BPMN.
/// </summary>
public class BpmBusiness
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Business ID.
    /// </summary>
    public string BusinessId { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Process code.
    /// </summary>
    public string ProcessCode { get; set; }

    /// <summary>
    /// Creator's user name.
    /// </summary>
    public string CreateUserName { get; set; }

    /// <summary>
    /// Creator's user ID.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// Logical delete flag (0: not deleted, 1: deleted).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
}