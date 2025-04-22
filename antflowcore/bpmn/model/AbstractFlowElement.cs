namespace antflowcore.bpmn.model;

public abstract class AbstractFlowElement : AFAbstractBaseElement, IAFExecutionListeners
{
    public string Name { get; set; }
    public string Documentation { get; set; }
    public List<AFActivitiListener> ExecutionListeners = new List<AFActivitiListener>();

    public List<AFActivitiListener> GetExecutionListeners()
    {
        return ExecutionListeners;
    }
}