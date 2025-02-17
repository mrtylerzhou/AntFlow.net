namespace antflowcore.bpmn;

public class MultiInstanceLoopCharacteristics
{
    public String InputDataItem { get; set; }
    public String LoopCardinality{ get; set; }
    public String CompletionCondition{ get; set; }
    public String ElementVariable{ get; set; }
    public String ElementIndexVariable{ get; set; }
    public bool Sequential{ get; set; }
}