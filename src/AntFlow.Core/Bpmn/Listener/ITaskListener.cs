using AntFlow.Core.Entity;

namespace AntFlow.Core.Bpmn.Listener;

public interface ITaskListener
{
    const string EVENTNAME_CREATE = "create";
    const string EVENTNAME_RE_SUBMIT = "resubmit";
    const string EVENTNAME_ASSIGNMENT = "assignment";
    const string EVENTNAME_COMPLETE = "complete";
    const string EVENTNAME_DELETE = "delete";
    void Notify(BpmAfTask delegateTask, string eventName);
}