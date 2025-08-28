namespace AntFlow.Core.Bpmn;

public class StartEvent : AbstractEvent
{
    public string Initiator { get; set; }
    public string FormCode { get; set; }
    public List<AFFormProperty> FormProperties { get; set; } = new();
}