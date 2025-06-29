using System;

namespace AntFlowCore.Entity;

public class BpmnConfNoticeTemplateDetail
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// BPMN Code.
    /// </summary>
    public string BpmnCode { get; set; }

    /// <summary>
    /// Notice template type.
    /// </summary>
    public int NoticeTemplateType { get; set; }

    /// <summary>
    /// Notice template detail.
    /// </summary>
    public string NoticeTemplateDetail { get; set; }

    /// <summary>
    /// Is deleted (0 for no, 1 for yes).
    /// </summary>
    public int IsDel { get; set; }

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