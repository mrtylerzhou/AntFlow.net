namespace AntFlow.Core.Bpmn;

public class AbstractActivity : AbstractFlowNode
{
    public string DefaultFlow { get; set; }
    public bool ForCompensation { get; set; }
    public MultiInstanceLoopCharacteristics LoopCharacteristics { get; set; }
}