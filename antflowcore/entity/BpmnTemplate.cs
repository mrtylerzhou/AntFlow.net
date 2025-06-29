using System;

namespace AntFlowCore.Entity;

/// <summary>
/// Represents the BPMN template configuration.
/// </summary>
public class BpmnTemplate
{
    /// <summary>
    /// Auto-incrementing ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Configuration ID.
    /// </summary>
    public long ConfId { get; set; }

    /// <summary>
    /// Node ID.
    /// </summary>
    public long? NodeId { get; set; }

    /// <summary>
    /// Event type.
    /// </summary>
    public int Event { get; set; }

    /// <summary>
    /// Inform types.
    /// </summary>
    public string Informs { get; set; }

    /// <summary>
    /// Specified employees.
    /// </summary>
    public string Emps { get; set; }

    /// <summary>
    /// Specified roles.
    /// </summary>
    public string Roles { get; set; }

    /// <summary>
    /// Specified functions.
    /// </summary>
    public string Funcs { get; set; }

    /// <summary>
    /// Template ID.
    /// </summary>
    public long TemplateId { get; set; }

    public string MessageSendType { get; set; }
    public string FormCode { get; set; }

    /// <summary>
    /// Deletion status (0 for no, 1 for yes).
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// User who created this record.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Last update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// User who last updated this record.
    /// </summary>
    public string UpdateUser { get; set; }
}