namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the configuration for BPMN node outside access.
/// </summary>
public class BpmnNodeOutSideAccessConf
{
    /// <summary>
    ///     Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     BPMN node ID.
    /// </summary>
    public long BpmnNodeId { get; set; }

    /// <summary>
    ///     Node mark.
    /// </summary>
    public string NodeMark { get; set; }

    /// <summary>
    ///     Sign type: 1 for all sign, 2 for or sign.
    /// </summary>
    public int? SignType { get; set; }

    /// <summary>
    ///     Remarks.
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    ///     Deletion flag (0 for normal, 1 for deleted).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    ///     Created by user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Updated by user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}