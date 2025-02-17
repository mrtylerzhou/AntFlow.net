namespace antflowcore.bpmn;

public abstract class AbstractFlowNode: AbstractFlowElement
{
   public List<AFSequenceFlow> IncomingFlows { get; set; } = new List<AFSequenceFlow>();
   public List<AFSequenceFlow> OutgoingFlows { get; set; } = new List<AFSequenceFlow>();
}