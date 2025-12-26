using System;

namespace AntFlowCore.Entity;

public class BpmnConfLfFormdataField
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
    /// Form data ID.
    /// </summary>
    public long FormDataId { get; set; }

    /// <summary>
    /// Field ID.
    /// </summary>
    public string FieldId { get; set; }

    /// <summary>
    /// Field name.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Field type.
    /// </summary>
    public int? FieldType { get; set; }

    /// <summary>
    /// Is condition field (0: No, 1: Yes).
    /// </summary>
    public int IsConditionField { get; set; }

    /// <summary>
    /// Delete flag (0 = false, 1 = true).
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }

    /// <summary>
    /// Created by user.
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Updated by user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    /// Update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}