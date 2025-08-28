namespace AntFlow.Core.Bpmn;

public abstract class AbstractFlowNode : AbstractFlowElement
{
    public List<AFSequenceFlow> IncomingFlows { get; set; } = new();
    public List<AFSequenceFlow> OutgoingFlows { get; set; } = new();
}