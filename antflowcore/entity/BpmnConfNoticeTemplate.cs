using System;

namespace AntFlowCore.Entity;

public class BpmnConfNoticeTemplate
{
    /// <summary>
    /// ID (Auto Increment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// BPMN Code
    /// </summary>
    public string BpmnCode { get; set; }

    /// <summary>
    /// Deletion Status (0: Normal, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Created By
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Creation Time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Updated By
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update Time
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}