namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the configuration for BPMN node personnel.
/// </summary>
public class BpmnNodePersonnelConf
{
    /// <summary>
    ///     Auto-increment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     BPMN node ID.
    /// </summary>
    public int BpmnNodeId { get; set; }

    /// <summary>
    ///     Sign type: 1 for co-sign, 2 for or-sign.
    /// </summary>
    public int SignType { get; set; }

    /// <summary>
    ///     Remarks.
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    ///     Deletion flag (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     Created by user (email prefix).
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Updated by user (email prefix).
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}