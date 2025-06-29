using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the configuration for external employees in BPMN node roles.
/// </summary>
public class BpmnNodeRoleOutsideEmpConf
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Node ID.
    /// </summary>
    public long NodeId { get; set; }

    /// <summary>
    /// Employee ID.
    /// </summary>
    public string EmplId { get; set; }

    /// <summary>
    /// Employee name.
    /// </summary>
    public string EmplName { get; set; }

    /// <summary>
    /// Deletion flag (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }

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