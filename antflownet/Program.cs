using antflowcore.aop;
using antflowcore.conf.di;
using antflowcore.conf.di.serviceregistration;
using antflowcore.conf.freesql;
using antflowcore.conf.json;
using antflowcore.conf.middleware;
using antflowcore.constant.enums;
using antflowcore.util;
using FreeSql;
using System.Text.Json;

namespace antflownet;

public class Program
{
    public static void Main(string[] args)
    {
        EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();
        var builder = WebApplication.CreateBuilder(args);
        ServiceCollectionHolder.SetServiceCollection(builder.Services);
        builder.Services.AddHttpContextAccessor();
        builder.Services.
            AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new StringOrIntConverter());
                options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new NullAbleBooleanJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new BooleanToIntJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new BooleanToNullableIntJsonConverter());
            });
        //freesqlset是创建的freesql的帮助类的方法名称
        builder.Services.FreeSqlSet(builder.Configuration);//注册freesql的相关服务
        builder.Services.AddFreeRepository();//freesql仓储
        builder.Services.AddScoped<UnitOfWorkManager>();//freesql uow
        builder.Services.AntFlowServiceSetUp();//注册AntFlow本身使用到的服务
        var app = builder.Build();
        ServiceProviderUtils.Initialize(app.Services);
        app.UseMiddleware<TransactionalMiddleware>();
        app.UseMiddleware<HeaderMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        //app.MapGet("/testvalue", () => service.testValue());
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action}");
        app.Run();
    }
}