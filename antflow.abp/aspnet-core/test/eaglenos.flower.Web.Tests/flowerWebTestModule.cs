using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using eaglenos.flower.EntityFrameworkCore;
using eaglenos.flower.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace eaglenos.flower.Web.Tests;

[DependsOn(
    typeof(flowerWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class flowerWebTestModule : AbpModule
{
    public flowerWebTestModule(flowerEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(flowerWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(flowerWebMvcModule).Assembly);
    }
}