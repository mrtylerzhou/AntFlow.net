using AntFlowCore.Bpmn.Bpmn.bpmn;

namespace AntFlowCore.Core.bpmnmodel;

public abstract class AbstractEvent: AbstractFlowNode
{
    public List<AbstractEventDefinition> EventDefinitions { get; set; } = new List<AbstractEventDefinition>();
}