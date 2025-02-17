namespace antflowcore.bpmn;

public class AFFormProperty
{
    public String Name { get; set; }
    public String Expression { get; set; }
    public String Variable { get; set; }
    public String Type { get; set; }
    public String DefaultExpression{ get; set; }
    public String DatePattern{ get; set; }
    public bool Readable { get; set; } = true;
    public bool Writeable { get; set; } = true;
    public bool required { get; set; }
}