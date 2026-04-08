namespace AntFlowCore.Base.bpmnmodel;

public abstract class AbstractEvent: AbstractFlowNode
{
    public List<AbstractEventDefinition> EventDefinitions { get; set; } = new List<AbstractEventDefinition>();
}