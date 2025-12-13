using Abp.Modules;
using Abp.Reflection.Extensions;
using antflow.abp.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace antflow.abp.Web.Host.Startup
{
    [DependsOn(
       typeof(AntflowWebCoreModule))]
    public class flowerWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public flowerWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(flowerWebHostModule).GetAssembly());
        }
    }
}
