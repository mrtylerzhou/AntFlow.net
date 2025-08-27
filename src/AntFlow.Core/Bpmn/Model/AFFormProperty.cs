namespace AntFlow.Core.Bpmn;

public class AFFormProperty
{
    public string Name { get; set; }
    public string Expression { get; set; }
    public string Variable { get; set; }
    public string Type { get; set; }
    public string DefaultExpression { get; set; }
    public string DatePattern { get; set; }
    public bool Readable { get; set; } = true;
    public bool Writeable { get; set; } = true;
    public bool required { get; set; }
}