using AntFlow.Core.Entity;

namespace AntFlow.Core.Bpmn.Listener;

public interface IExecutionListener
{
    public const string EVENTNAME_START = "start";
    public const string EVENTNAME_END = "end";
    public const string EVENTNAME_TAKE = "take";
    void Notify(BpmAfExecution execution, string eventName);
}