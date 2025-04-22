namespace antflowcore.entity;

public class AFProcess
{
    private readonly List<object> _flowElements = new List<object>();

    public void AddFlowElement(object flowElement)
    {
        _flowElements.Add(flowElement);
    }
}