using Microsoft.Extensions.DependencyInjection;

namespace antflowcore.conf.di;

public static class ServiceCollectionHolder
{
    public static IServiceCollection Services { get; private set; }

    public static void SetServiceCollection(IServiceCollection services)
    {
        Services = services;
    }
}
