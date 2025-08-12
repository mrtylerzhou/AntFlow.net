using antflowcore.aop;
using antflowcore.conf.di;
using antflowcore.conf.freesql;
using antflowcore.conf.json;
using antflowcore.conf.middleware;
using antflowcore.conf.serviceregistration;
using antflowcore.constant.enus;
using antflowcore.util;
using FreeSql;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

namespace antflownet;

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "antflownet", Version = "v1" });

            Assembly[] assemblies =
            [
                Assembly.Load("antflowcore"),
                Assembly.Load("antflownet"),
            ];
            foreach (var assembly in assemblies)
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            }
        });
        //解决跨域
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", bd => bd
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
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
        WebApplication app = builder.Build();
        app.Services.AddFreeSqlFluentConfig();
        app.MapOpenApi();
        app.UseCors("CorsPolicy");//解决跨域
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

        app.MapGet("/", () => $"Hello Antflow!");
        app.Run();
    }
}