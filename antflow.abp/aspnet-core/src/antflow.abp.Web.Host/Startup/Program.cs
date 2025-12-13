using Abp.AspNetCore.Dependency;
using Abp.Dependency;
using antflowcore.constant.enus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace antflow.abp.Web.Host.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();

            CreateHostBuilder(args).Build().Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseCastleWindsor(IocManager.Instance.IocContainer);
    }
}
