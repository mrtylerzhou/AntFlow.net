namespace AntFlowCore.Base.entity;

/// <summary>
/// 流程表单字段变更审计记录.
/// 对应 Java 版 jimuoffice 的 BpmProcessAudit / t_bpm_process_audit.
/// 每次审批(同意/重提/加批/协办)在 OnConsentData 写入新值之前, 记录所有表单字段的旧值/新值.
/// 字段未变化也记录(便于无差别展示当时状态).
/// </summary>
public class BpmProcessAudit
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程编号
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 表单编码
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 字段名(fieldId). 低代码为 inputId(如 input68478), DIY 为 vo 属性名.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// 字段 label(展示给业务用户).
    /// 低代码从 t_bpmn_conf_lf_formdata_field.field_name 读取; DIY 无 label 概念, 保持 null.
    /// </summary>
    public string FieldLabel { get; set; }

    /// <summary>
    /// 旧值(字符串形式; 对象/集合 JSON 序列化)
    /// </summary>
    public string OldValue { get; set; }

    /// <summary>
    /// 新值(字符串形式; 对象/集合 JSON 序列化)
    /// </summary>
    public string NewValue { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 任务名称(节点名)
    /// </summary>
    public string TaskName { get; set; }

    /// <summary>
    /// 任务定义key(节点元素ID)
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 变更人(登录empId)
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 变更人姓名(审批时快照, 用于审计溯源).
    /// CreateUser 是 empId, 这里存当时的登录人姓名, 避免后续员工改名/查询联表.
    /// </summary>
    public string CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
