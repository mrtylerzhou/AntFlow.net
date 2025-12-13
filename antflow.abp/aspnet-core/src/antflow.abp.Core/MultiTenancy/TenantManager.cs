using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using antflow.abp.Authorization.Users;
using antflow.abp.Editions;

namespace antflow.abp.MultiTenancy;

public class TenantManager : AbpTenantManager<Tenant, User>
{
    public TenantManager(
        IRepository<Tenant> tenantRepository,
        IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
        EditionManager editionManager,
        IAbpZeroFeatureValueStore featureValueStore)
        : base(
            tenantRepository,
            tenantFeatureRepository,
            editionManager,
            featureValueStore)
    {
    }
}
