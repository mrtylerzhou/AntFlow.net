namespace antflowcore.bpmn;

public abstract class ExecutionListener
{
    String EVENTNAME_START = "start";
    String EVENTNAME_END = "end";
    String EVENTNAME_TAKE = "take";

    public abstract void Notify(ExecutionEntity execution);
}