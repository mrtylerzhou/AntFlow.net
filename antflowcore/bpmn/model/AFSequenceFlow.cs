namespace antflowcore.bpmn.model;

public class AFSequenceFlow : AbstractFlowElement
{
    public AFSequenceFlow()
    { }

    public AFSequenceFlow(string sourceRef, string targetRef)
    {
        SourceRef = sourceRef;
        TargetRef = targetRef;
    }

    public string SourceRef { get; set; }
    public string TargetRef { get; set; }
    public string ConditionExpression { get; set; }

    public AFSequenceFlow Clone()
    {
        AFSequenceFlow self = (AFSequenceFlow)MemberwiseClone();
        return self;
    }
}