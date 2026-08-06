# 性能优化建议

## 概述

AntFlowCore 作为企业级工作流引擎，在处理高并发审批场景时需要合理的性能调优。本文档从数据库、应用、缓存、前端等多个维度提供优化建议。

## 性能指标参考

| 指标 | 目标值 | 说明 |
|------|--------|------|
| 接口响应时间 | < 200ms | 普通查询接口 |
| 流程启动时间 | < 500ms | 包含节点计算 |
| 审批操作响应 | < 300ms | 同意/拒绝操作 |
| 并发用户数 | > 500 | 单实例支持 |
| 数据库连接数 | < 100 | 连接池上限 |

## 数据库优化

### 1. 索引优化

```sql
-- 流程配置查询索引
CREATE INDEX idx_bpmn_conf_tenant ON bpmn_conf(tenant_id, is_del);
CREATE INDEX idx_bpmn_conf_formcode ON bpmn_conf(form_code);

-- 任务查询索引（最高频）
CREATE INDEX idx_task_assignee_type ON bpm_af_task(assignee, type, status);
CREATE INDEX idx_task_process_number ON bpm_af_task(process_number);
CREATE INDEX idx_task_create_time ON bpm_af_task(create_time);

-- 执行实例索引
CREATE INDEX idx_execution_proc_def ON bpm_af_execution(proc_def_id);
CREATE INDEX idx_execution_business ON bpm_af_execution(business_id);

-- 历史数据索引
CREATE INDEX idx_historic_proc_def ON historic_process_instance(proc_def_id);
CREATE INDEX idx_historic_start_time ON historic_process_instance(start_time);
```

### 2. 慢查询分析

启用 FreeSql 的 SQL 监控：

```csharp
.UseMonitorCommand(cmd =>
{
    if (cmd.ElapsedMilliseconds > 500)
    {
        logger.LogWarning("Slow SQL ({Elapsed}ms): {Sql}", 
            cmd.ElapsedMilliseconds, cmd.CommandText);
    }
})
```

### 3. 分页查询优化

```csharp
// 推荐：使用 FreeSql 的分页方法
public ResultAndPage<TaskMgmtVO> FindPcProcessList(
    PageDto pageDto, TaskMgmtVO taskMgmtVO)
{
    var query = fsql.Select<BpmAfTask>()
        .Where(o => o.Assignee == taskMgmtVO.UserId)
        .Where(o => o.Status == taskMgmtVO.Status);
    
    // 获取总记录数
    long count = query.Count();
    
    // 分页获取数据
    var list = query.Page(pageDto.PageNo, pageDto.PageSize)
        .OrderByDescending(o => o.CreateTime)
        .ToList();
    
    return new ResultAndPage<TaskMgmtVO>
    {
        Total = count,
        List = list.Select(o => ConvertToVo(o)).ToList()
    };
}
```

### 4. 读写分离

高并发场景下配置读写分离：

```json
{
  "FreeSql": {
    "ConnectionString": "Server=master;...",
    "SlaveConnections": [
      "Server=slave1;...",
      "Server=slave2;..."
    ]
  }
}
```

## 应用层优化

### 1. 对象映射优化

使用表达式树缓存避免反射：

```csharp
// 推荐：使用 AutoMapper 或手动映射
private static readonly Expression<Func<BpmAfTask, TaskMgmtVO>> TaskMap =
    task => new TaskMgmtVO
    {
        Id = task.Id,
        ProcessNumber = task.ProcessNumber,
        TaskName = task.TaskName,
        // ... 其他字段
    };

public TaskMgmtVO ConvertToVo(BpmAfTask task)
{
    return taskMgmtVO.Compile().Invoke(task);
}
```

### 2. 异步编程

```csharp
// 推荐：IO 密集型操作使用 async/await
public async Task<Result<List<BpmVerifyInfoVo>>> GetVerifyInfoAsync(
    string processNumber)
{
    var list = await fsql.Select<BpmVerifyInfo>()
        .Where(o => o.ProcessNumber == processNumber)
        .OrderBy(o => o.VerifyTime)
        .ToListAsync();
    
    return ResultHelper.Success(list);
}
```

### 3. 批量操作

```csharp
// 不推荐：逐条插入
foreach (var item in items)
{
    await fsql.Insert(item).ExecuteIdentityAsync();
}

// 推荐：批量插入
await fsql.Insert(items).ExecuteIdentityAsync();
```

### 4. 避免 N+1 查询

```csharp
// 不推荐：循环中查询
var tasks = await fsql.Select<BpmAfTask>().ToListAsync();
foreach (var task in tasks)
{
    task.Conf = await fsql.Select<BpmnConf>()
        .Where(o => o.Id == task.BpmnConfId)
        .FirstAsync();
}

// 推荐：一次性加载
var tasks = await fsql.Select<BpmAfTask>()
    .Include(o => o.Conf)
    .ToListAsync();
```

## 缓存策略

### 1. 多级缓存架构

```
请求 → 本地缓存(L1) → Redis(L2) → 数据库
         ↑                  ↑
    1-5分钟过期        5-30分钟过期
```

### 2. 缓存实现示例

```csharp
public class ProcessConfigCache
{
    private readonly IMemoryCache _localCache;
    private readonly IDistributedCache _distributedCache;
    private readonly IFreeSql _freeSql;
    
    private const string CacheKeyPrefix = "bpmn:conf:";
    private static readonly TimeSpan LocalCacheExpiration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan RedisCacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<BpmnConf> GetConfigAsync(long configId)
    {
        var cacheKey = $"{CacheKeyPrefix}{configId}";
        
        // L1: 本地缓存
        if (_localCache.TryGetValue(cacheKey, out BpmnConf config))
        {
            return config;
        }
        
        // L2: Redis
        var redisData = await _distributedCache.GetStringAsync(cacheKey);
        if (redisData != null)
        {
            config = JsonSerializer.Deserialize<BpmnConf>(redisData);
            SetLocalCache(cacheKey, config);
            return config;
        }
        
        // DB: 数据库查询
        config = await _freeSql.Select<BpmnConf>()
            .Where(o => o.Id == configId)
            .FirstAsync();
        
        if (config != null)
        {
            await SetDistributedCache(cacheKey, config);
            SetLocalCache(cacheKey, config);
        }
        
        return config;
    }

    public async Task InvalidateAsync(long configId)
    {
        var cacheKey = $"{CacheKeyPrefix}{configId}";
        _localCache.Remove(cacheKey);
        await _distributedCache.RemoveAsync(cacheKey);
    }
}
```

### 3. 缓存数据选择

**适合缓存的数据**：
- 流程模板配置（变更频率低）
- 用户基础信息
- 字典/枚举数据
- 审批人规则配置

**不适合缓存的数据**：
- 任务状态（实时性要求高）
- 审批意见（写入频繁）
- 计数器类数据

## 前端优化

### 1. 流程图渲染优化

```javascript
// 虚拟滚动：只渲染可视区域的节点
const visibleNodes = computed(() => {
    return allNodes.value.slice(startIndex.value, endIndex.value);
});

// 使用 Canvas 替代 SVG（节点数 > 50 时）
if (nodes.length > 50) {
    renderer = new CanvasRenderer();
} else {
    renderer = new SvgRenderer();
}
```

### 2. 表单懒加载

```javascript
// 按步骤加载表单组件
const loadFormComponent = async (componentType) => {
    const { default: Component } = await import(
        `@/components/form/${componentType}.vue`
    );
    return Component;
};
```

### 3. 接口请求优化

```javascript
// 防抖搜索
const debouncedSearch = debounce(async (keyword) => {
    const res = await fetchFormCodes(keyword);
    formCodes.value = res.data;
}, 300);

// 请求合并
const batchRequest = debounce(async (ids) => {
    const res = await fetchTaskDetails(ids);
    // 批量更新
}, 50, { maxWait: 100 });
```

## JVM/运行时优化

### 1. GC 配置（.NET）

```json
{
  "runtimeOptions": {
    "configProperties": {
      "System.GC.Server": true,
      "System.GC.HeapHardLimit": 2147483648
    }
  }
}
```

### 2. 线程池配置

```json
{
  "ThreadPool": {
    "MinThreads": 100,
    "MinCompletionPortThreads": 100
  }
}
```

## 监控与调优

### 1. 性能计数器

```csharp
// 注册性能计数器
public class WorkflowMetrics
{
    private readonly Counter<int> _taskCompletedCounter;
    private readonly Histogram<double> _taskProcessDuration;
    
    public WorkflowMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("AntFlowCore.Workflow");
        _taskCompletedCounter = meter.CreateCounter<int>(
            "tasks.completed", description: "已完成的任务数");
        _taskProcessDuration = meter.CreateHistogram<double>(
            "task.duration", "ms", "任务处理耗时");
    }
    
    public void RecordTaskCompletion(string taskType)
    {
        _taskCompletedCounter.Add(1, 
            new KeyValuePair<string, object>("type", taskType));
    }
}
```

### 2. APM 集成

```csharp
// OpenTelemetry 配置
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddFreeSqlInstrumentation()  // 自定义 FreeSql 埋点
            .AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter();
    });
```

## 压测建议

### 1. 压测场景设计

| 场景 | 并发数 | 持续时间 | 目标 |
|------|--------|---------|------|
| 流程启动 | 100 | 10min | TPS > 50 |
| 审批操作 | 200 | 10min | TPS > 100 |
| 任务查询 | 500 | 5min | RT < 200ms |
| 混合场景 | 300 | 30min | 无内存泄漏 |

### 2. 压测工具

```bash
# 使用 NBomber 进行压测
dotnet run --project tests/AntFlowCore.LoadTest

# 或 wrk
wrk -t12 -c400 -d30s --latency http://localhost:8080/api/todoList
```

## 优化检查清单

- [ ] 数据库表已添加必要索引
- [ ] 慢查询日志已开启并分析
- [ ] 连接池大小已合理配置
- [ ] 热数据已添加缓存层
- [ ] IO 操作已使用异步
- [ ] 大循环中的数据库查询已优化
- [ ] 批量操作替代逐条操作
- [ ] 流程图大数据量使用 Canvas 渲染
- [ ] 前端组件已按需加载
- [ ] 性能监控已接入
- [ ] GC 已配置为 Server 模式
- [ ] 压测已通过目标指标
