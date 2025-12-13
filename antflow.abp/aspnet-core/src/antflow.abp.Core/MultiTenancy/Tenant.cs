using Abp.MultiTenancy;
using antflow.abp.Authorization.Users;

namespace antflow.abp.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
