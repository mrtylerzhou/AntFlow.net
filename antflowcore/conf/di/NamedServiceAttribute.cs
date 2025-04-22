namespace antflowcore.conf.di;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class NamedServiceAttribute : Attribute
{
    public string ServiceName { get; }

    public NamedServiceAttribute(string serviceName)
    {
        ServiceName = serviceName;
    }
}