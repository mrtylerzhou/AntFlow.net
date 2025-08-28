namespace AntFlow.Core.Configuration.DependencyInjection;

public static class ServiceCollectionHolder
{
    public static IServiceCollection Services { get; private set; }

    public static void SetServiceCollection(IServiceCollection services)
    {
        Services = services;
    }
}