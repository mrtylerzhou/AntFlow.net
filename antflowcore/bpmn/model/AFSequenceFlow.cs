namespace antflowcore.bpmn;

public class AFSequenceFlow: AbstractFlowElement
{
    public AFSequenceFlow(){}
    public AFSequenceFlow(String sourceRef, String targetRef) {
        this.SourceRef = sourceRef;
        this.TargetRef = targetRef;
    }
    public String SourceRef { get; set; }
    public String TargetRef{ get; set; }
    public string ConditionExpression { get; set; }

    public AFSequenceFlow Clone()
    {
        AFSequenceFlow self = (AFSequenceFlow)this.MemberwiseClone();
        return self;
    }
}