using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the configuration for BPMN node roles.
/// </summary>
public class BpmnNodeRoleConf
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// BPMN node ID.
    /// </summary>
    public long BpmnNodeId { get; set; }

    /// <summary>
    /// Role ID.
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// Sign type (1 for all sign, 2 for OR sign).
    /// </summary>
    public int? SignType { get; set; }

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