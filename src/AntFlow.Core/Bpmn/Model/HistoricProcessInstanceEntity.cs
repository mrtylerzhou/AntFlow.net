namespace AntFlow.Core.Bpmn;

public class HistoricProcessInstanceEntity
{
    public string Id { get; set; }
    public string ProcessInstanceId { get; set; }
    public string EndActivityId { get; set; }
    public string BusinessKey { get; set; }
    public string StartUserId { get; set; }
    public string StartActivityId { get; set; }
    public string SuperProcessInstanceId { get; set; }
    public string TenantId { get; set; } = TaskEntity.DEFAULT_NO_TENANT_ID;
    public string Name { get; set; }
    public string LocalizedName { get; set; }
    public string Description { get; set; }
    public string LocalizedDescription { get; set; }
}