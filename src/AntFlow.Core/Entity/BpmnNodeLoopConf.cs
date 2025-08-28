namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the configuration for loop in BPMN node.
/// </summary>
public class BpmnNodeLoopConf
{
    /// <summary>
    ///     Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Node ID.
    /// </summary>
    public long? BpmnNodeId { get; set; }

    /// <summary>
    ///     Loop end type (used for extensibility, can be ignored if not needed).
    /// </summary>
    public int? LoopEndType { get; set; }

    /// <summary>
    ///     Number of loop levels.
    /// </summary>
    public int? LoopNumberPlies { get; set; }

    /// <summary>
    ///     End person.
    /// </summary>
    public string LoopEndPerson { get; set; }

    /// <summary>
    ///     Staff IDs not participating in the loop.
    /// </summary>
    public string NoparticipatingStaffIds { get; set; }

    /// <summary>
    ///     End grade.
    /// </summary>
    public int? LoopEndGrade { get; set; }

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