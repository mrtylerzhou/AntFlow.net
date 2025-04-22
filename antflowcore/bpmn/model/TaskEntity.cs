namespace antflowcore.bpmn.model;

public class TaskEntity
{
    public const string DELETE_REASON_COMPLETED = "completed";
    public const string DELETE_REASON_DELETED = "deleted";
    public const string DEFAULT_NO_TENANT_ID = "";

    public string Id { get; private set; }
    public int Revision { get; set; }

    public string Owner { get; set; }
    public string Assignee { get; set; }
    public string AssigneeName { get; set; }
    public string InitialAssignee { get; set; }

    public string ParentTaskId { get; set; }

    public string Name { get; set; }
    public string LocalizedName { get; set; }
    public string Description { get; set; }
    public string LocalizedDescription { get; set; }
    public int Priority { get; set; } = DEFAULT_PRIORITY;
    public DateTime? CreateTime { get; set; }
    public DateTime? DueDate { get; set; }
    public int SuspensionState { get; set; }
    public string Category { get; set; }
    public string CategoryName { get; set; }

    public string ExecutionId { get; set; }
    public ExecutionEntity Execution { get; set; }

    public string ProcessInstanceId { get; set; }
    public ExecutionEntity ProcessInstance { get; set; }

    public string ProcessDefinitionId { get; set; }

    public TaskDefinition TaskDefinition { get; set; }
    public string TaskDefinitionKey { get; set; }
    public string FormKey { get; set; }

    public bool IsDeleted { get; set; }

    public string EventName { get; set; }

    public string TenantId { get; set; } = DEFAULT_NO_TENANT_ID;

    public const int DEFAULT_PRIORITY = 50;
}