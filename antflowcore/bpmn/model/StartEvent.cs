namespace antflowcore.bpmn;

public class StartEvent: AbstractEvent
{
   public String Initiator { get; set; }
   public String FormCode{ get; set; }
   public List<AFFormProperty> FormProperties { get; set; } = new List<AFFormProperty>();
}