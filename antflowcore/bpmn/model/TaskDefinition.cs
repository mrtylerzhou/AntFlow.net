namespace antflowcore.bpmn.model;

public class TaskDefinition
{
    public string key { get; set; }

    // task listeners
    public Dictionary<string, List<TaskListener>> taskListeners = new Dictionary<string, List<TaskListener>>();
}