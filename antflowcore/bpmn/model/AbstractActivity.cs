namespace antflowcore.bpmn;

public class AbstractActivity: AbstractFlowNode
{
    public String DefaultFlow { get; set; }
    public bool ForCompensation{ get; set; }
    public MultiInstanceLoopCharacteristics LoopCharacteristics{ get; set; }
}