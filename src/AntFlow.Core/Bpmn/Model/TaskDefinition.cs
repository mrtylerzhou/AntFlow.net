namespace AntFlow.Core.Bpmn;

public class TaskDefinition
{
    // task listeners
    public Dictionary<string, List<TaskListener>> taskListeners = new();
    public string key { get; set; }
}