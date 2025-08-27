namespace AntFlow.Core.Entity;

/// <summary>
///     ?????????????
/// </summary>
public class BpmAfTask
{
    /// <summary>
    ///     ????
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     ????·Ú
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    ///     ?????? ID
    /// </summary>
    public string ExecutionId { get; set; }

    /// <summary>
    ///     ??????? ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    ///     ??????? ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     ????????
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     ?????? ID
    /// </summary>
    public string ParentTaskId { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    public string TaskDefKey { get; set; }

    public string NodeId { get; set; }
    public int NodeType { get; set; }

    /// <summary>
    ///     ??????????
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    ///     ??????????
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     ??????????
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    ///     ??????
    /// </summary>
    public string Delegation { get; set; }

    /// <summary>
    ///     ?????
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    ///     ???????
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    ///     ??????
    /// </summary>
    public int? SuspensionState { get; set; }

    /// <summary>
    ///     ?? ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    ///     ?????
    /// </summary>
    public string FormKey { get; set; }

    /// <summary>
    ///     ????????
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     ??????????????
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    ///     ????????????????????
    /// </summary>
    public bool IsNextNodeSignUp { get; set; }
}

public static class BpmAfTaskExtensions
{
    public static BpmAfTaskInst ToInst(this BpmAfTask bpmAfTask)
    {
        BpmAfTaskInst inst = new()
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
            TenantId = bpmAfTask.TenantId
        };
        return inst;
    }
}