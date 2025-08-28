using AntFlow.Core.Service.Interface;

namespace AntFlow.Core.Util;

public static class MultiTenantUtil
{
    /// <summary>
    ///     获取当前租户ID
    /// </summary>
    public static string GetCurrentTenantId()
    {
        ITenantIdHolder tenantIdHolder = ServiceProviderUtils.GetService<ITenantIdHolder>();
        if (tenantIdHolder == null)
        {
            throw new InvalidOperationException("无法获取 TenantIdHolder 服务，请检查依赖注入配置");
        }

        return tenantIdHolder.GetCurrentTenantId();
    }

    /// <summary>
    ///     检查是否启用严格的多租户模式，在严格模式下会进行更严格的租户隔离检查
    ///     通过配置文件中的antflow.multitenant.strict参数控制，true表示启用严格模式
    /// </summary>
    public static bool StrictTenantMode()
    {
        IConfiguration configuration = ServiceProviderUtils.GetService<IConfiguration>();
        string? value = configuration["antflow.multitenant.strict"];
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}