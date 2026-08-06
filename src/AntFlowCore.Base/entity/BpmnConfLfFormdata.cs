namespace AntFlowCore.Base.entity;

public class BpmnConfLfFormdata
{
    /// <summary>
    /// Primary key ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程配置ID（独立表单为NULL；内联表单指向所属流程的conf id）
    /// </summary>
    public long? BpmnConfId { get; set; }

    /// <summary>
    /// 独立表单家族标识（同族各版本共享；内联表单为NULL）
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    /// 独立表单显示名（内联表单为NULL）
    /// </summary>
    public string FormName { get; set; }

    /// <summary>
    /// 是否当前生效版本 0否 1是（仅独立表单使用；内联表单恒为0）
    /// </summary>
    public int EffectiveStatus { get; set; }

    /// <summary>
    /// Form data (in JSON format).
    /// </summary>
    public string Formdata { get; set; }

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