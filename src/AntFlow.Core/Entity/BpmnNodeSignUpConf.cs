namespace AntFlow.Core.Entity;

/// <summary>
///     BPMN Node Sign Up Configuration
/// </summary>
public class BpmnNodeSignUpConf
{
    /// <summary>
    ///     Auto Increment ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Node ID
    /// </summary>
    public long BpmnNodeId { get; set; }

    /// <summary>
    ///     After Sign-Up Way (1: back to sign-up person, 2: not back to sign-up person)
    /// </summary>
    public int AfterSignUpWay { get; set; }

    /// <summary>
    ///     Sign-Up Type (1: sequential, 2: unordered, 3: or)
    /// </summary>
    public int SignUpType { get; set; }

    /// <summary>
    ///     Remark
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    ///     Deletion Status (0: normal, 1: deleted)
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     Created By
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     Creation Time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Updated By
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     Update Time
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}