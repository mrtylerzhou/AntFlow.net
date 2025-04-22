namespace antflowcore.bpmn.model;

public class AFActivitiListener
{
    public string Evt { get; set; }
    public string ImplementationType { get; set; }
    public string Implementation { get; set; }
    public List<AFFieldExtension> FieldExtensions { get; set; } = new List<AFFieldExtension>();
}