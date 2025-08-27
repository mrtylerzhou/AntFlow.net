using AntFlow.Core.Aop;
using AntFlow.Core.Conf.FreeSql;
using AntFlow.Core.Conf.Json;
using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Configuration.Middleware;
using AntFlow.Core.Configuration.ServiceRegistration;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using FreeSql;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

namespace AntFlow.Web;

public class Program
{
    public static void Main(string[] args)
    {
        EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ServiceCollectionHolder.SetServiceCollection(builder.Services);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers().AddAFApplicationComponents(); //Add Custom  Mvc Controller
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AntFlow.Web", Version = "v1" });

            Assembly[] assemblies =
            [
                Assembly.Load("AntFlow.Core"),
                Assembly.Load("AntFlow.Web")
            ];
            foreach (Assembly? assembly in assemblies)
            {
                string? xmlFile = $"{assembly.GetName().Name}.xml";
                string? xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            }
        });
        //跨域配置
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", bd => bd
                .SetIsOriginAllowed(host => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        builder.Services.AddControllers()
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
        //freesqlset是创建和配置freesql的包装器的方法集合
        builder.Services.FreeSqlSet(builder.Configuration); //注册freesql相关方法
        builder.Services.AddFreeRepository(); //freesql仓储
        builder.Services.AddScoped<UnitOfWorkManager>(); //freesql uow
        builder.Services.AntFlowServiceSetUp(); //注册AntFlow项目使用到的服务
        WebApplication app = builder.Build();
        app.Services.AddFreeSqlFluentConfig();
        app.MapOpenApi();
        app.UseCors("CorsPolicy"); //跨域配置
        ServiceProviderUtils.Initialize(app.Services);
        app.UseMiddleware<TransactionalMiddleware>();
        app.UseMiddleware<HeaderMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        //app.MapGet("/testvalue", () => service.testValue());
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", () => "Hello Antflow!");
        app.Run();
    }
}