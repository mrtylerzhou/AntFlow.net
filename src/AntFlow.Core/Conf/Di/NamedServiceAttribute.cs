namespace AntFlow.Core.Configuration.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class NamedServiceAttribute : Attribute
{
    public NamedServiceAttribute(string serviceName)
    {
        ServiceName = serviceName;
    }

    public string ServiceName { get; }
}