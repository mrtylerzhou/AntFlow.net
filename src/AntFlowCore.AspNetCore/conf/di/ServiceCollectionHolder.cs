using Microsoft.Extensions.DependencyInjection;

namespace AntFlowCore.AspNetCore.AspNetCore.conf.di;

public static class ServiceCollectionHolder
{
    public static IServiceCollection Services { get; private set; }

    public static void SetServiceCollection(IServiceCollection services)
    {
        Services = services;
    }
}
