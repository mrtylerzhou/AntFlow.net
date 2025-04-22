namespace antflowcore.bpmn;

public abstract class TaskListener
{
    public String EVENTNAME_CREATE = "create";
    public String EVENTNAME_ASSIGNMENT = "assignment";
    public String EVENTNAME_COMPLETE = "complete";
    public String EVENTNAME_DELETE = "delete";
    public String EVENTNAME_ALL_EVENTS = "all";
    public abstract void Notify(TaskEntity delegateTask);
}