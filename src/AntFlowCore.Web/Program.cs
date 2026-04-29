using System.Reflection;
using System.Text.Json;
using AntFlowCore.Abstraction.aop;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.AspNetCore.conf.di;
using AntFlowCore.AspNetCore.conf.middleware;
using AntFlowCore.Base.conf.json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Engine.Abstraction.conf.di.serviceregistration;
using AntFlowCore.Engine.Abstraction.conf.sqlsugar;
using Microsoft.OpenApi;

public class Program
{
    public static void Main(string[] args)
    {
        EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ServiceCollectionHolder.SetServiceCollection(builder.Services);
        ServiceProviderUtils.Initialize(builder.Services);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers().AddAFApplicationComponents();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "antflownet", Version = "v1" });

            Assembly[] assemblies =
            [
                Assembly.Load("AntFlowCore.Abstraction"),
                Assembly.Load("AntFlowCore.Api"),
                Assembly.Load("AntFlowCore.AspNetCore"),
                Assembly.Load("AntFlowCore.Base"),
                Assembly.Load("AntFlowCore.Bpmn"),
                Assembly.Load("AntFlowCore.Engine"),
                Assembly.Load("AntFlowCore.Persist"),
                Assembly.Load("AntFlowCore.Persist.api"),
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
               options.JsonSerializerOptions.Converters.Add(new GlobalNullableIntConverter());
            });
        builder.Services.SqlSugarSet(builder.Configuration);
        builder.Services.AntFlowServiceSetUp(builder.Configuration);
        WebApplication app = builder.Build();
        app.Services.AddSqlSugarFluentConfig();
        app.MapOpenApi();
        app.UseCors("CorsPolicy");
        ServiceProviderUtils.Initialize(app.Services);
        app.UseMiddleware<TransactionalMiddleware>();
        app.UseMiddleware<HeaderMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
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
