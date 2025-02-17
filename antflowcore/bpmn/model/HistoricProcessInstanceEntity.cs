namespace antflowcore.bpmn;

public class HistoricProcessInstanceEntity
{
   public string Id { get; set; }
   public string ProcessInstanceId { get; set; }
   public String EndActivityId { get; set; }
   public String BusinessKey { get; set; }
   public String StartUserId { get; set; }
   public String StartActivityId { get; set; }
   public String SuperProcessInstanceId { get; set; }
   public String TenantId { get; set; } =TaskEntity.DEFAULT_NO_TENANT_ID ;
   public String Name { get; set; }
   public String LocalizedName { get; set; }
   public String Description { get; set; }
   public String LocalizedDescription { get; set; }
}