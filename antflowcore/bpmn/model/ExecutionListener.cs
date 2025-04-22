namespace antflowcore.bpmn.model;

public abstract class ExecutionListener
{
    private string EVENTNAME_START = "start";
    private string EVENTNAME_END = "end";
    private string EVENTNAME_TAKE = "take";

    public abstract void Notify(ExecutionEntity execution);
}