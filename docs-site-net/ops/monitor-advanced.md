# 监控和日志

## 概述

AntFlowCore 提供完整的可观测性方案，包括日志记录、性能监控、健康检查和告警机制。本文档介绍如何配置和运维监控系统。

## 日志架构

```
┌─────────────────────────────────────────────────────┐
│                   应用层                              │
│  API Controllers → Domain Services → Repositories    │
└─────────────────┬───────────────────────────────────┘
                  │ Serilog
                  ▼
┌─────────────────────────────────────────────────────┐
│              日志管道 (Serilog)                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────┐   │
│  │ Console  │  │  File    │  │ Seq / Elasticsearch│  │
│  │ 控制台输出 │  │ 文件日志  │  │  结构化日志存储    │  │
│  └──────────┘  └──────────┘  └──────────────────┘   │
└─────────────────────────────────────────────────────┘
```

## Serilog 配置

### 1. 安装 NuGet 包

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
dotnet add package Serilog.Enrichers.Thread
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Process
```

### 2. appsettings.json 配置

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning",
        "System.Net.Http.HttpClient": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30",
          "fileSizeLimitBytes": 52428800,
          "rollOnFileSizeLimit": true,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341",
          "apiKey": "your-seq-api-key",
          "restrictedToMinimumLevel": "Information"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithThreadId",
      "WithMachineName",
      "WithEnvironmentName",
      "WithProcessId"
    ],
    "Properties": {
      "Application": "AntFlowCore",
      "Environment": "Production"
    }
  }
}
```

### 3. Program.cs 注册

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 使用 Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.WithProperty("Environment", 
            context.HostingEnvironment.EnvironmentName);
});

var app = builder.Build();

// 请求日志中间件
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = 
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null) return LogEventLevel.Error;
        if (elapsed > 1000) return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };
});

app.Run();
```

## 结构化日志

### 1. 日志模板

```csharp
// 推荐：使用结构化日志
logger.LogInformation(
    "流程 {ProcessNumber} 由 {UserId} 在 {NodeId} 节点完成 {Action} 操作",
    processNumber, userId, nodeId, action);

// 不推荐：字符串拼接
logger.LogInformation($"流程 {processNumber} 由 {userId} 完成操作");
```

### 2. 日志作用域

```csharp
// 为一批日志添加公共属性
using (logger.BeginScope(new Dictionary<string, object>
{
    ["ProcessNumber"] = processNumber,
    ["UserId"] = userId,
    ["TenantId"] = tenantId
}))
{
    logger.LogInformation("开始处理流程");
    // ... 业务逻辑
    logger.LogInformation("流程处理完成");
}
```

### 3. 自定义日志事件类型

```csharp
public static class LogEvents
{
    public static readonly LogEvent ProcessStart = new LogEvent(
        LogLevel.Information, 
        EventIds.ProcessStart, 
        "流程 {ProcessNumber} 启动");
    
    public static readonly LogEvent TaskComplete = new LogEvent(
        LogLevel.Information, 
        EventIds.TaskComplete, 
        "任务 {TaskId} 完成，操作: {Action}");
    
    public static readonly LogEvent ProcessError = new LogEvent(
        LogLevel.Error, 
        EventIds.ProcessError, 
        "流程 {ProcessNumber} 执行异常: {ErrorMessage}");
}

public static class EventIds
{
    public const int ProcessStart = 1001;
    public const int TaskComplete = 1002;
    public const int ProcessError = 9001;
}
```

## 健康检查

### 1. 基础健康检查

```csharp
// 注册健康检查
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddDbConnectionString(
        builder.Configuration["FreeSql:ConnectionString"],
        name: "database",
        tags: new[] { "db", "sql" })
    .AddRedis(
        builder.Configuration["Redis:ConnectionString"],
        name: "redis",
        tags: new[] { "cache" });
```

### 2. 自定义健康检查

```csharp
public class FreeSqlHealthCheck : IHealthCheck
{
    private readonly IFreeSql _freeSql;

    public FreeSqlHealthCheck(IFreeSql freeSql)
    {
        _freeSql = freeSql;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 执行简单查询验证数据库连接
            var result = await _freeSql.Ado.ExecuteCommandAsync(
                "SELECT 1");
            
            return result > 0 
                ? HealthCheckResult.Healthy("数据库连接正常")
                : HealthCheckResult.Unhealthy("数据库查询异常");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "数据库连接失败", ex);
        }
    }
}

// 注册
builder.Services.AddHealthChecks()
    .AddCheck<FreeSqlHealthCheck>("freesql");
```

### 3. 健康检查端点

```csharp
// 基础端点
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// 详细端点（需要授权）
app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                exception = e.Value.Exception?.Message
            })
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});
```

## 性能监控

### 1. OpenTelemetry 集成

```bash
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.Runtime
```

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("AntFlowCore"))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.Filter = req => 
                    !req.Request.Path.StartsWithSegments("/health");
            })
            .AddHttpClientInstrumentation()
            .AddSource("AntFlowCore.Workflow")
            .AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(
                    builder.Configuration["Otlp:Endpoint"]);
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter("AntFlowCore.Workflow")
            .AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(
                    builder.Configuration["Otlp:Endpoint"]);
            });
    });
```

### 2. 自定义指标

```csharp
public class WorkflowMeters
{
    private readonly Meter _meter;
    private readonly Counter<long> _tasksCreated;
    private readonly Counter<long> _tasksCompleted;
    private readonly Histogram<double> _taskDuration;
    private readonly ObservableGauge<int> _activeProcesses;

    public WorkflowMeters() : this(null) { }

    public WorkflowMeters(IMeterFactory meterFactory)
    {
        _meter = meterFactory?.Create("AntFlowCore.Workflow") 
            ?? new Meter("AntFlowCore.Workflow");
        
        _tasksCreated = _meter.CreateCounter<long>(
            "workflow.tasks.created",
            description: "创建的任务总数");
        
        _tasksCompleted = _meter.CreateCounter<long>(
            "workflow.tasks.completed",
            description: "完成的任务总数");
        
        _taskDuration = _meter.CreateHistogram<double>(
            "workflow.task.duration",
            "ms",
            "任务处理耗时");
        
        _activeProcesses = _meter.CreateObservableGauge(
            "workflow.processes.active",
            () => GetActiveProcessCount(),
            description: "当前活跃的流程实例数");
    }

    public void RecordTaskCreated(string processType)
    {
        _tasksCreated.Add(1, 
            new KeyValuePair<string, object>("type", processType));
    }

    public void RecordTaskCompleted(string processType, 
        TimeSpan duration)
    {
        _tasksCompleted.Add(1, 
            new KeyValuePair<string, object>("type", processType));
        _taskDuration.Record(duration.TotalMilliseconds,
            new KeyValuePair<string, object>("type", processType));
    }

    private int GetActiveProcessCount()
    {
        // 从数据库或缓存获取活跃流程数
        return 0;
    }
}
```

## 告警配置

### 1. 基于日志的告警规则

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Conditional",
        "Args": {
          "condition": "RequestElapsedGreaterThan(5000)",
          "configureSink": [
            {
              "Name": "File",
              "Args": {
                "path": "logs/slow-requests-.log",
                "rollingInterval": "Day"
              }
            }
          ]
        }
      }
    ]
  }
}
```

### 2. 自定义告警中间件

```csharp
public class AlertMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AlertMiddleware> _logger;

    public AlertMiddleware(RequestDelegate next, 
        ILogger<AlertMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "未捕获异常: {Path}", 
                context.Request.Path);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // 慢请求告警
            if (stopwatch.ElapsedMilliseconds > 5000)
            {
                _logger.LogWarning(
                    "慢请求告警: {Method} {Path} 耗时 {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
            
            // 错误率告警
            if (context.Response.StatusCode >= 500)
            {
                _logger.LogError(
                    "服务错误: {Method} {Path} 返回 {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode);
            }
        }
    }
}
```

## 日志分析

### 1. Seq 查询示例

```sql
-- 查询最近1小时的错误日志
SELECT * FROM Stream
WHERE Level = 'Error' and @Timestamp > SubtractHours(@now, 1)
ORDER BY @Timestamp DESC

-- 查询流程处理耗时分布
select Bin(Elapsed, 100) as Bucket, Count(*) as Count
from Stream
where Elapsed is not null
group by Bin(Elapsed, 100)

-- 查询最慢的10个请求
select top 10 RequestPath, Elapsed, @Timestamp
from Stream
where Elapsed is not null
order by Elapsed desc
```

### 2. Kibana 可视化

```json
{
  "dashboard": {
    "title": "AntFlowCore 监控面板",
    "panels": [
      {
        "title": "请求量趋势",
        "type": "line",
        "query": "application:AntFlowCore",
        "interval": "1m"
      },
      {
        "title": "错误率",
        "type": "gauge",
        "query": "level:Error",
        "threshold": { "warning": 1, "critical": 5 }
      },
      {
        "title": "平均响应时间",
        "type": "stat",
        "aggregation": "avg",
        "field": "elapsed"
      }
    ]
  }
}
```

## 日志保留策略

| 日志类型 | 保留时间 | 存储位置 | 说明 |
|---------|---------|---------|------|
| 应用日志 | 30天 | 本地文件 / Seq | 日常运维 |
| 错误日志 | 90天 | Seq / ES | 问题排查 |
| 审计日志 | 1年 | 独立存储 | 合规要求 |
| 访问日志 | 7天 | 本地文件 | 安全分析 |
| 性能日志 | 30天 | Prometheus | 容量规划 |

## 故障排查日志级别

```csharp
// 开发环境：详细日志
"MinimumLevel": { "Default": "Debug" }

// 测试环境：信息日志
"MinimumLevel": { "Default": "Information" }

// 生产环境：警告日志
"MinimumLevel": { "Default": "Warning" }

// 紧急排查：临时开启详细日志
"MinimumLevel": { 
    "Default": "Debug",
    "Override": {
        "AntFlowCore.Engine": "Trace"
    }
}
```
