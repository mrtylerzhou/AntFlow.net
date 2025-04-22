using antflowcore.entity;

namespace antflowcore.bpmn.listener;

public interface IExecutionListener
{
    public const String EVENTNAME_START = "start";
    public const String EVENTNAME_END = "end";
    public const String EVENTNAME_TAKE = "take";
    void Notify(BpmAfExecution execution,string eventName);
}