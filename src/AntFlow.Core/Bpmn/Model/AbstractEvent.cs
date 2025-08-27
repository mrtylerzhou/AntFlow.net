namespace AntFlow.Core.Bpmn;

public abstract class AbstractEvent : AbstractFlowNode
{
    public List<AbstractEventDefinition> EventDefinitions { get; set; } = new();
}