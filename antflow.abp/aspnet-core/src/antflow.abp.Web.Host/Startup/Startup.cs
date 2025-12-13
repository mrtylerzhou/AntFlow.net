using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using antflowcore.aop;
using antflowcore.conf.di;
using antflowcore.conf.freesql;
using antflowcore.conf.json;
using antflowcore.conf.middleware;
using antflowcore.conf.serviceregistration;
using antflowcore.controller;
using antflowcore.util;
using Castle.Facilities.Logging;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using antflow.abp.Configuration;
using antflow.abp.Identity;

namespace antflow.abp.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private const string _apiVersion = "v1";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ServiceCollectionHolder.SetServiceCollection(services);
            //MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddAFApplicationComponents(); //Add Custom  Mvc Controller;


            // 或者通过反射扫描注册
            var thirdPartyAssembly = typeof(LowCodeFlowController).Assembly;
            var controllerTypes = thirdPartyAssembly.GetTypes()
                .Where(t => t.Name.EndsWith("Controller") &&
                           t.GetCustomAttribute<RouteAttribute>() != null)
                .ToList();

            foreach (var controllerType in controllerTypes)
            {
                services.AddTransient(controllerType);
            }

            //services.AddTransient<LowCodeFlowController>();
            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                         //.WithOrigins(
                         //    // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                         //    _appConfiguration["App:CorsOrigins"]
                         //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                         //        .Select(o => o.RemovePostFix("/"))
                         //        .ToArray()
                         //)
                         .SetIsOriginAllowed((host) => true)
                        //.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            ConfigureSwagger(services);

            // Configure Abp and Dependency Injection
            services.AddAbpWithoutCreatingServiceProvider<flowerWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config"
                    )
                )
            );


            //flower
            services.AddHttpContextAccessor();
            //services.AddControllers().AddAFApplicationComponents(); //Add Custom  Mvc Controller
            //services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "antflownet", Version = "v1" });

            //    Assembly[] assemblies =
            //    [
            //        Assembly.Load("antflowcore"),
            //    Assembly.Load("antflownet"),
            //];
            //    foreach (var assembly in assemblies)
            //    {
            //        var xmlFile = $"{assembly.GetName().Name}.xml";
            //        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            //        if (File.Exists(xmlPath))
            //        {
            //            c.IncludeXmlComments(xmlPath);
            //        }
            //    }
            //});
      
            //services.
            //    AddControllers()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
            //        options.JsonSerializerOptions.Converters.Add(new StringOrIntConverter());
            //        options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
            //        options.JsonSerializerOptions.Converters.Add(new NullAbleBooleanJsonConverter());
            //        options.JsonSerializerOptions.Converters.Add(new BooleanToIntJsonConverter());
            //        options.JsonSerializerOptions.Converters.Add(new BooleanToNullableIntJsonConverter());
            //    });
            //freesqlset是创建的freesql的帮助类的方法名称
            services.FreeSqlSet(_appConfiguration);//注册freesql的相关服务
            services.AddFreeRepository();//freesql仓储
            services.AddScoped<UnitOfWorkManager>();//freesql uow
            services.AntFlowServiceSetUp();//注册AntFlow本身使用到的服务
       

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {  //freesql
            app.ApplicationServices.AddFreeSqlFluentConfig();
            // app.MapOpenApi();
            // app.UseCors("CorsPolicy");//解决跨域
            ServiceProviderUtils.Initialize(app.ApplicationServices);
            app.UseMiddleware<TransactionalMiddleware>();
            app.UseMiddleware<HeaderMiddleware>();
           app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAbpRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                // specifying the Swagger JSON endpoint.
                options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"antflow abp API {_apiVersion}");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("antflow.abp.Web.Host.wwwroot.swagger.ui.index.html");
                options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.
            });  



          
            //app.MapGet("/testvalue", () => service.testValue());
            // app.MapControllers();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.MapGet("/", () => $"Hello Antflow!");
            //app.Run();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_apiVersion, new OpenApiInfo
                {
                    Version = _apiVersion,
                    Title = "antflow abp API",
                    Description = "antflow",
                    // uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "flower",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/aspboilerplate"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/LICENSE.md"),
                    }
                });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                //add summaries to swagger
                bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                if (canShowSummaries)
                {
                    var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                    options.IncludeXmlComments(hostXmlPath);

                    var applicationXml = $"antflow.abp.Application.xml";
                    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                    options.IncludeXmlComments(applicationXmlPath);

                    var webCoreXmlFile = $"antflow.abp.Web.Core.xml";
                    var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                    options.IncludeXmlComments(webCoreXmlPath);
                }
            });
        }
    }
}
