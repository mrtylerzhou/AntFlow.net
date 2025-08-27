namespace Microsoft.Extensions.DependencyInjection;

public static class AFApplicationExtension
{
    public static IMvcBuilder AddAFApplicationComponents(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(AFApplicationExtension).Assembly);
        return builder;
    }
}