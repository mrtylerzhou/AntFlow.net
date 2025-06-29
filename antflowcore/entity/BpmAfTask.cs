using System;

namespace antflowcore.entity;

/// <summary>
/// 流程任务实体类
/// </summary>
public class BpmAfTask
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 修订版本
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    /// 执行实例 ID
    /// </summary>
    public string ExecutionId { get; set; }

    /// <summary>
    /// 流程实例 ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    /// 流程定义 ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 父任务 ID
    /// </summary>
    public string ParentTaskId { get; set; }

    /// <summary>
    /// 任务定义键
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 任务所有者
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// 任务受托人
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    /// 受托人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    /// 委托信息
    /// </summary>
    public string Delegation { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 到期时间
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 任务类别
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 挂起状态
    /// </summary>
    public int? SuspensionState { get; set; }

    /// <summary>
    /// 租户 ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 表单键
    /// </summary>
    public string FormKey { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 流程编号（非持久化）
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 是否下一节点会签（非持久化）
    /// </summary>
    public bool IsNextNodeSignUp { get; set; }
    
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