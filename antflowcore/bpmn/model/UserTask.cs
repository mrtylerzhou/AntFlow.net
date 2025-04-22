namespace antflowcore.bpmn;

public class UserTask: AbstractTask
{
   public String Assignee { get; set; }
   public String Owner { get; set; }
   public String Priority { get; set; }
   public String FormCode { get; set; }
   public String DueDate { get; set; }
   public String BusinessCalendarName{ get; set; }
   public String Category { get; set; }
   public String ExtensionId { get; set; }
   public List<String> CandidateUsers { get; set; } = new List<string>();
   public List<String> CandidateGroups { get; set; } = new List<string>();
   public List<AFFormProperty> FormProperties { get; set; } = new List<AFFormProperty>();
   public List<AFActivitiListener> TaskListeners { get; set; } = new List<AFActivitiListener>();
   public String SkipExpression { get; set; }
}