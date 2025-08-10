using antflowcore.service.interf;
using Microsoft.Extensions.Configuration;

namespace antflowcore.util;

public static class MultiTenantUtil
{
    

    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    public static string GetCurrentTenantId()
    {
        

        ITenantIdHolder tenantIdHolder = ServiceProviderUtils.GetService<ITenantIdHolder>();
        if (tenantIdHolder == null)
            throw new InvalidOperationException("未找到 TenantIdHolder 服务，请检查依赖注入配置。");

        return tenantIdHolder.GetCurrentTenantId();
    }

    /// <summary>
    /// 严格模式，租户只能使用自身的配置（默认租户可使用全部）
    /// 非严格模式，租户可共用配置，但业务数据（包括流程引擎数据）仍然分离。
    /// </summary>
    public static bool StrictTenantMode()
    {
        IConfiguration configuration = ServiceProviderUtils.GetService<IConfiguration>();
        var value = configuration["antflow:multitenant:strict"];
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
