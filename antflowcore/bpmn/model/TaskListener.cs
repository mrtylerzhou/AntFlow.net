namespace antflowcore.bpmn.model;

public abstract class TaskListener
{
    public string EVENTNAME_CREATE = "create";
    public string EVENTNAME_ASSIGNMENT = "assignment";
    public string EVENTNAME_COMPLETE = "complete";
    public string EVENTNAME_DELETE = "delete";
    public string EVENTNAME_ALL_EVENTS = "all";

    public abstract void Notify(TaskEntity delegateTask);
}