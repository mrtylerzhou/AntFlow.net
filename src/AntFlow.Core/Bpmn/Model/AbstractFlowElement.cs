namespace AntFlow.Core.Bpmn;

public abstract class AbstractFlowElement : AFAbstractBaseElement, IAFExecutionListeners
{
    public List<AFActivitiListener> ExecutionListeners = new();
    public string Name { get; set; }
    public string Documentation { get; set; }

    public List<AFActivitiListener> GetExecutionListeners()
    {
        return ExecutionListeners;
    }
}