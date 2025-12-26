using System;

namespace AntFlowCore.Entity;

public class BpmnApproveRemind
{
    /// <summary>
    /// ID (Auto Increment)
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Configuration ID
    /// </summary>
    public long ConfId { get; set; }

    /// <summary>
    /// Node ID
    /// </summary>
    public long NodeId { get; set; }

    /// <summary>
    /// Template ID
    /// </summary>
    public long? TemplateId { get; set; }

    /// <summary>
    /// Days
    /// </summary>
    public string Days { get; set; }

    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    /// <summary>
    /// Creation Time
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Created By
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Update Time
    /// </summary>
    public DateTime? UpdateTime { get; set; }=DateTime.Now;

    /// <summary>
    /// Updated By
    /// </summary>
    public string UpdateUser { get; set; }
}