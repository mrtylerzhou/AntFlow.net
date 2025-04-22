namespace antflowcore.bpmn.model;

public class MultiInstanceLoopCharacteristics
{
    public string InputDataItem { get; set; }
    public string LoopCardinality { get; set; }
    public string CompletionCondition { get; set; }
    public string ElementVariable { get; set; }
    public string ElementIndexVariable { get; set; }
    public bool Sequential { get; set; }
}