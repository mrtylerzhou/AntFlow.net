namespace antflowcore.bpmn.model;

public abstract class AbstractEvent : AbstractFlowNode
{
    public List<AbstractEventDefinition> EventDefinitions { get; set; } = new List<AbstractEventDefinition>();
}