using antflowcore.entity;

namespace antflowcore.bpmn.listener;

public interface ITaskListener
{
    const String EVENTNAME_CREATE = "create";
    const String EVENTNAME_RE_SUBMIT = "resubmit";
    const String EVENTNAME_ASSIGNMENT = "assignment";
    const String EVENTNAME_COMPLETE = "complete";
    const String EVENTNAME_DELETE = "delete";

    void Notify(BpmAfTask delegateTask, string eventName);
}