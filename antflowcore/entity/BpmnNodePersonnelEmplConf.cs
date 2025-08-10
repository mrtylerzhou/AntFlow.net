using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the configuration for BPMN node personnel employees.
/// </summary>
public class BpmnNodePersonnelEmplConf
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// BPMN node personnel ID.
    /// </summary>
    public int BpmnNodePersonneId { get; set; }

    /// <summary>
    /// Employee ID.
    /// </summary>
    public string EmplId { get; set; }

    /// <summary>
    /// Employee name.
    /// </summary>
    public string EmplName { get; set; }

    /// <summary>
    /// Remarks.
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// Deletion flag (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
    /// <summary>
    /// Created by user (email prefix).
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Updated by user (email prefix).
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}