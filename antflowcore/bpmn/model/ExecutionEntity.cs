namespace antflowcore.bpmn;

public class ExecutionEntity
{
  
    public String TenantId = TaskEntity.DEFAULT_NO_TENANT_ID;
    public String Name { get; set; }
    public string ProcessInstanceId { get; set; }
    public String Description { get; set; }
    public String LocalizedName { get; set; }
    public String LocalizedDescription { get; set; }
    public ExecutionEntity ProcessInstance { get; set; }
    public ExecutionEntity Parent { get; set; }
    public List<ExecutionEntity> Executions { get; set; }
    public ExecutionEntity SuperExecution { get; set; }
    public ExecutionEntity SubProcessInstance { get; set; }
}