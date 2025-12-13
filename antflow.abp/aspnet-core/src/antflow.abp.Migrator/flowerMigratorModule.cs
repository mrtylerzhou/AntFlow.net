using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using antflow.abp.Configuration;
using antflow.abp.EntityFrameworkCore;
using antflow.abp.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace antflow.abp.Migrator;

[DependsOn(typeof(flowerEntityFrameworkModule))]
public class flowerMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public flowerMigratorModule(flowerEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(flowerMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            flowerConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(flowerMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
