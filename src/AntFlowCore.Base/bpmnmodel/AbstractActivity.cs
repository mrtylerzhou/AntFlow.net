using AntFlowCore.Bpmn.Bpmn.bpmn;

namespace AntFlowCore.Core.bpmnmodel;

public class AbstractActivity: AbstractFlowNode
{
    public String DefaultFlow { get; set; }
    public bool ForCompensation{ get; set; }
    public MultiInstanceLoopCharacteristics LoopCharacteristics{ get; set; }
}