using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// 流程任务实体类
/// </summary>
[Table(Name = "bpm_af_task")]
[Index("AF_IDX_TASK_CREATE", nameof(CreateTime))] // 创建时间索引
[Index("AF_IDX_PROCINSTID", nameof(ProcInstId))] // 流程实例 ID 索引
public class BpmAfTask
{
    /// <summary>
    /// 主键
    /// </summary>
    [Column(Name = "id", IsPrimary = true)]
    public string Id { get; set; }

    /// <summary>
    /// 修订版本
    /// </summary>
    [Column(Name = "rev")]
    public int? Rev { get; set; }

    /// <summary>
    /// 执行实例 ID
    /// </summary>
    [Column(Name = "execution_id")]
    public string ExecutionId { get; set; }

    /// <summary>
    /// 流程实例 ID
    /// </summary>
    [Column(Name = "proc_inst_id")]
    public string ProcInstId { get; set; }

    /// <summary>
    /// 流程定义 ID
    /// </summary>
    [Column(Name = "proc_def_id")]
    public string ProcDefId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Column(Name = "name")]
    public string Name { get; set; }

    /// <summary>
    /// 父任务 ID
    /// </summary>
    [Column(Name = "parent_task_id")]
    public string ParentTaskId { get; set; }

    /// <summary>
    /// 任务定义键
    /// </summary>
    [Column(Name = "task_def_key")]
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 任务所有者
    /// </summary>
    [Column(Name = "owner")]
    public string Owner { get; set; }

    /// <summary>
    /// 任务受托人
    /// </summary>
    [Column(Name = "assignee")]
    public string Assignee { get; set; }

    /// <summary>
    /// 受托人姓名
    /// </summary>
    [Column(Name = "assignee_name")]
    public string AssigneeName { get; set; }

    /// <summary>
    /// 委托信息
    /// </summary>
    [Column(Name = "delegation")]
    public string Delegation { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [Column(Name = "priority")]
    public int? Priority { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "create_time")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 到期时间
    /// </summary>
    [Column(Name = "due_date")]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 任务类别
    /// </summary>
    [Column(Name = "category")]
    public string Category { get; set; }

    /// <summary>
    /// 挂起状态
    /// </summary>
    [Column(Name = "suspension_state")]
    public int? SuspensionState { get; set; }

    /// <summary>
    /// 租户 ID
    /// </summary>
    [Column(Name = "tenant_id")]
    public string TenantId { get; set; }

    /// <summary>
    /// 表单键
    /// </summary>
    [Column(Name = "form_key")]
    public string FormKey { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    [Column(Name = "description")]
    public string Description { get; set; }

    [Column(IsIgnore = true)]
    public string ProcessNumber { get; set; }
}

public static class BpmAfTaskExtensions
{
    public static BpmAfTaskInst ToInst(this BpmAfTask bpmAfTask)
    {
        BpmAfTaskInst inst = new BpmAfTaskInst()
        {
            Id = bpmAfTask.Id,
            Name = bpmAfTask.Name,
            ProcDefId = bpmAfTask.ProcDefId,
            ProcInstId = bpmAfTask.ProcInstId,
            TaskDefKey = bpmAfTask.TaskDefKey,
            ExecutionId = bpmAfTask.ExecutionId,
            Owner = bpmAfTask.Owner,
            Assignee = bpmAfTask.Assignee,
            AssigneeName = bpmAfTask.AssigneeName,
            StartTime = bpmAfTask.CreateTime,
            Description = bpmAfTask.Description,
            FormKey = bpmAfTask.FormKey,
        };
        return inst;
    }
}