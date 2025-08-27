namespace AntFlow.Core.Entity;

public class AFProcess
{
    private readonly List<object> _flowElements = new();

    public void AddFlowElement(object flowElement)
    {
        _flowElements.Add(flowElement);
    }
}