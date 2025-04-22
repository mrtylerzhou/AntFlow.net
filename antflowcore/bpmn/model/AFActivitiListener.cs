namespace antflowcore.bpmn;

public class AFActivitiListener
{
    public String Evt { get; set; }
    public String ImplementationType{ get; set; }
    public String Implementation{ get; set; }
    public List<AFFieldExtension> FieldExtensions { get; set; } = new List<AFFieldExtension>();
}