using Microsoft.Extensions.DependencyInjection;

namespace AntFlowCore.AspNetCore.conf.di;
public static class AFApplicationExtension
{
    public static IMvcBuilder AddAFApplicationComponents(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(AFApplicationExtension).Assembly);
        return builder;
    }
}
