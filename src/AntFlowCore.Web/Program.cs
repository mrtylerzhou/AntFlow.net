using System.Reflection;
using System.Text.Json;
using AntFlowCore.Abstraction.aop;
using AntFlowCore.Abstraction.Orm.sqlsugar;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.AspNetCore.conf.di;
using AntFlowCore.AspNetCore.conf.middleware;
using AntFlowCore.Base.conf.json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Engine.Abstraction.conf.di.serviceregistration;
using Microsoft.OpenApi;

namespace AntFlowCore.Web;

public class Program
{
    public static void Main(string[] args)
    {
        EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ServiceCollectionHolder.SetServiceCollection(builder.Services);
        ServiceProviderUtils.Initialize(builder.Services);
        builder.Services.AddHttpContextAccessor(); 
        builder.Services.AddControllers().AddAFApplicationComponents(); //Add Custom  Mvc Controller
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "antflowcore", Version = "v1" });

            Assembly[] assemblies =
            [
                Assembly.Load("AntFlowCore.Api"),
                Assembly.Load("AntFlowCore.Business"),
                Assembly.Load("AntFlowCore.VirtualNode"),
                Assembly.Load("AntFlowCore.Bpmn"),
                Assembly.Load("AntFlowCore.Engine"),
                Assembly.Load("AntFlowCore.Engine.Abstraction"),
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
               //fixed https://gitee.com/antswarm/antflowcore/issues/IDI57X
               options.JsonSerializerOptions.Converters.Add(new GlobalNullableIntConverter());
            });
        //SqlSugarSet是创建的SqlSugar的帮助类的方法名称
        builder.Services.SqlSugarSet(builder.Configuration);//注册SqlSugar的相关服务
        builder.Services.AntFlowServiceSetUp(builder.Configuration);//注册AntFlow本身使用到的服务
        WebApplication app = builder.Build();
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
