namespace AntFlow.Core.Bpmn;

public class UserTask : AbstractTask
{
    public string Assignee { get; set; }
    public string Owner { get; set; }
    public string Priority { get; set; }
    public string FormCode { get; set; }
    public string DueDate { get; set; }
    public string BusinessCalendarName { get; set; }
    public string Category { get; set; }
    public string ExtensionId { get; set; }
    public List<string> CandidateUsers { get; set; } = new();
    public List<string> CandidateGroups { get; set; } = new();
    public List<AFFormProperty> FormProperties { get; set; } = new();
    public List<AFActivitiListener> TaskListeners { get; set; } = new();
    public string SkipExpression { get; set; }
}