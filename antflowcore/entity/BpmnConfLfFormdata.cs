using System;

namespace AntFlowCore.Entity;

public class BpmnConfLfFormdata
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// BPMN configuration ID.
    /// </summary>
    public long BpmnConfId { get; set; }

    /// <summary>
    /// Form data (in JSON format).
    /// </summary>
    public string Formdata { get; set; }

    /// <summary>
    /// Delete flag (0 = false, 1 = true).
    /// </summary>
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
    /// <summary>
    /// Created by user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Updated by user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}