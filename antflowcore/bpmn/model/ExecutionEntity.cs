namespace antflowcore.bpmn.model;

public class ExecutionEntity
{
    public string TenantId = TaskEntity.DEFAULT_NO_TENANT_ID;
    public string Name { get; set; }
    public string ProcessInstanceId { get; set; }
    public string Description { get; set; }
    public string LocalizedName { get; set; }
    public string LocalizedDescription { get; set; }
    public ExecutionEntity ProcessInstance { get; set; }
    public ExecutionEntity Parent { get; set; }
    public List<ExecutionEntity> Executions { get; set; }
    public ExecutionEntity SuperExecution { get; set; }
    public ExecutionEntity SubProcessInstance { get; set; }
}