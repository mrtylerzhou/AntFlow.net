using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using antflow.abp.Authorization;

namespace antflow.abp;

[DependsOn(
    typeof(flowerCoreModule),
    typeof(AbpAutoMapperModule))]
public class flowerApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<flowerAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(flowerApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
