# AntFlowCore 事务处理详解 - 从 AOP 注解到 UnitOfWork

## 前言

AntFlowCore 使用 FreeSql 作为 ORM 框架，事务处理自然也基于 FreeSql 的 `UnitOfWork`，同时通过 AOP 框架提供了非常简洁的 `[Transactional]` 注解用法。本文深入解析 AntFlowCore 的事务实现机制，以及如何在你的业务代码中正确使用事务。

## 技术选型：Rougamo + FreeSql UnitOfWork

AntFlowCore 选择了：
- **Rougamo（肉夹馍）**：零侵入 AOP 框架，用来实现 `[Transactional]` 注解
- **FreeSql.UnitOfWork**：FreeSql 官方提供的工作单元管理器

这种组合非常简洁，不需要引入复杂的 AOP 框架，就可以实现声明式事务。

## 核心源码解析

### 1. TransactionalAttribute 实现

我们先看 `antflowcore/aop/TransactionalAttribute.cs` 的核心代码：

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class TransactionalAttribute : Rougamo.MoAttribute
{
    public Propagation Propagation { get; set; } = Propagation.Required;
    public IsolationLevel IsolationLevel { get; set; }

    static AsyncLocal<IServiceProvider> m_ServiceProvider = new AsyncLocal<IServiceProvider>();
    
    public static void SetServiceProvider(IServiceProvider serviceProvider) 
        => m_ServiceProvider.Value = serviceProvider;

    IUnitOfWork _uow;
    
    public override void OnEntry(MethodContext context)
    {
        // 方法进入时，开启 UnitOfWork
        var uowManager = (UnitOfWorkManager)m_ServiceProvider.Value
            .GetService(typeof(UnitOfWorkManager));
        _uow = uowManager.Begin(this.Propagation, this.IsolationLevel);
    }

    public override void OnExit(MethodContext context)
    {
        // 方法退出时，根据是否异常决定提交还是回滚
        if (typeof(Task).IsAssignableFrom(context.ReturnType))
            ((Task)context.ReturnValue).ContinueWith(t => _OnExit());
        else 
            _OnExit();

        void _OnExit()
        {
            try
            {
                if (context.Exception == null) 
                    _uow.Commit();
                else 
                    _uow.Rollback();
            }
            finally
            {
                _uow.Dispose();
            }
        }
    }
}
```

设计非常简洁优雅：
- 继承 `Rougamo.MoAttribute`，Rougamo 会自动织入 AOP
- 方法进入时，从 DI 获取 `UnitOfWorkManager`，开启一个新的 UnitOfWork
- 方法退出时，如果没有异常，提交事务；如果有异常，回滚事务
- 使用 `AsyncLocal` 保存 `IServiceProvider`，异步环境也没问题

### 2. 中间件初始化 ServiceProvider

`antflowcore/conf/middleware/TransactionalMiddleware.cs`:

```csharp
public class TransactionalMiddleware : IMiddleware
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionalMiddleware(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        TransactionalAttribute.SetServiceProvider(_serviceProvider);
        await next(context);
    }
}
```

每一次请求，都会把当前请求的 `IServiceProvider` 设置给 `TransactionalAttribute`，这样 AOP 就能拿到 `UnitOfWorkManager` 了。

所以在 `Program.cs` 中，你会看到：

```csharp
app.UseMiddleware<TransactionalMiddleware>();
```

这就是完整的事务处理链路了。

## 传播级别支持

AntFlowCore 直接支持 FreeSql 的传播级别：

```csharp
public enum Propagation
{
    Required, // 如果已有事务，加入；没有，新建一个
    RequiresNew, // 不管有没有，都新建一个
    Mandatory, // 必须已有事务，否则抛异常
    Never, // 不能有事务，否则抛异常
    NotSupported, // 不支持事务，挂起当前事务
    Supports, // 支持，如果有就加入
}
```

默认传播级别是 `Required`，这也是最常用的。

用法：

```csharp
// 默认传播级别 Required
[Transactional]
public async Task MyMethod()
{
    // ...
}

// 指定传播级别和隔离级别
[Transactional(Propagation = Propagation.RequiresNew, IsolationLevel = IsolationLevel.ReadCommitted)]
public async Task MyMethodRequiresNew()
{
    // ...
}
```

## 使用方式：声明式事务，一句话就够了

你只需要给方法加上 `[Transactional]` 注解，就自动开启事务了：

```csharp
public class MyProcessService
{
    private readonly IProcessStarterService _processStarter;
    private readonly IBusinessRepository _businessRepo;

    [Transactional]
    public async Task StartMyProcessAsync(CreateBusinessDto dto)
    {
        // 1. 保存业务数据
        var business = new BusinessEntity
        {
            Title = dto.Title,
            Amount = dto.Amount
        };
        await _businessRepo.InsertAsync(business);

        // 2. 启动流程
        await _processStarter.StartProcessAsync(
            "my_process_key",
            business.Id.ToString(),
            new Dictionary<string, object>
            {
                { "amount", dto.Amount },
                { "title", dto.Title }
            });

        // 这里不需要提交，AOP 会自动提交
        // 如果抛出异常，AOP 自动回滚
    }
}
```

就是这么简单！业务数据保存 + 流程启动，都在同一个事务里，要么都成功，要么都回滚，保证数据一致性。

## 不同场景的传播级别选择

### 场景一：普通业务方法，需要事务

```csharp
[Transactional] // 默认 Required，足够了
public async Task SaveAsync()
{
    // ...
}
```

### 场景二：嵌套调用，外层已经有事务

```csharp
[Transactional]
public async Task OuterAsync()
{
    // 开启事务
    await InnerAsync();
    // ...
}

[Transactional(Propagation = Propagation.Required)] // 加入外层事务，没问题
public async Task InnerAsync()
{
    // ...
}
```

整个外层和内层在同一个事务中，任何一层抛出异常，全部回滚。

### 场景三：不管外层有没有事务，我都要新开事务

比如记录日志，就算主业务失败，日志也要保存：

```csharp
[Transactional(Propagation = Propagation.RequiresNew)]
public async Task LogAsync(string message)
{
    // 即使外层事务回滚，这里的日志还是会提交
    await _logRepo.InsertAsync(new Log { Message = message });
}
```

### 场景四：只读方法，不需要事务

```csharp
[Transactional(Propagation = Propagation.NotSupported)]
public async Task<List<ProcessVo>> QueryAsync()
{
    // 只读查询，不需要事务，提升性能
    return await _processRepo.QueryListAsync();
}
```

## 手动控制事务

如果你不喜欢 AOP，想手动控制事务，也完全可以：

```csharp
public class MyService
{
    private readonly UnitOfWorkManager _uowManager;
    private readonly IProcessTaskService _taskService;
    private readonly IBusinessService _businessService;

    public MyService(UnitOfWorkManager uowManager)
    {
        _uowManager = uowManager;
    }

    public async Task CompleteTaskAsync(int taskId)
    {
        using var uow = _uowManager.Begin();
        try
        {
            // 更新业务状态
            await _businessService.UpdateStatusAsync(taskId);
            
            // 完成任务
            await _taskService.CompleteAsync(taskId);
            
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}
```

两种方式都可以，根据你的喜好选择。

## 事务和事件系统的配合

当你在事件处理中操作数据库，事务会自动工作吗？

是的，因为事件处理本身就在请求处理流程中，`TransactionalMiddleware` 已经设置好了 `IServiceProvider`，所以你直接用 `[Transactional]` 就行：

```csharp
public class ProcessCompletedHandler : IFlowEventHandler<ProcessCompletedEvent>
{
    [Transactional]
    public async Task HandleAsync(ProcessCompletedEvent @event)
    {
        // 更新业务状态，自动在事务中
        await _businessRepo.UpdateStatusAsync(
            @event.ProcessInstance.BusinessKey, 
            Status.Approved);
    }
}
```

如果事件处理抛出异常，事务自动回滚。

## 常见问题

### 1. 私有方法能开事务吗？

Rougamo 默认不支持私有方法，所以 `[Transactional]` 应该放在 public 方法上。

### 2. 同一个类内部方法调用，AOP 生效吗？

这是 AOP 的经典问题，如果直接 `this.MyMethod()` 调用，AOP 不会生效，因为走的是原生调用，不是代理调用。

解决方法：
- 把方法放到另一个类
- 或者用 `_serviceProvider.GetService<MyClass>().MyMethod()` 调用

### 3. 异常被捕获了，事务还会回滚吗？

不会。只有异常抛到 AOP 层面，AOP 才知道出错了，才会回滚。如果你捕获了异常吃掉了，AOP 认为方法正常执行，会提交事务。

错误示例：

```csharp
[Transactional]
public async Task MyMethod()
{
    try
    {
        // ...
    }
    catch (Exception ex)
    {
        _logger.Error(ex);
        // 异常被吃掉了，AOP 看不到异常，会提交事务
    }
}
```

正确做法：如果你处理了异常，但还是需要回滚，重新抛出：

```csharp
catch (Exception ex)
{
    _logger.Error(ex);
    throw; // 重新抛出，让 AOP 回滚
}
```

### 4. AntFlowCore 内部操作会自动加入我的事务吗？

会的。因为 AntFlowCore 用的也是同一个 `UnitOfWorkManager`，所以你的业务代码操作和 AntFlowCore 操作都在同一个事务里，保证数据一致性。

这就是为什么我们推荐在你的业务方法上加 `[Transactional]`，业务数据和流程数据一起提交一起回滚。

### 5. 可以跨多个数据库事务吗？

FreeSql 本身支持跨库事务，AntFlowCore 也支持，需要你配置好分布式事务（或者使用支持跨库事务的数据库）。

### 6. 为什么要用 Rougamo 而不是其他 AOP 框架？

Rougamo 非常轻量，零配置，只需要给方法加上注解就可以用，不需要像 Castle 那样动态代理，也不需要改项目配置，对代码零侵入，非常适合这种简单的声明式事务场景。

## 性能怎么样？

Rougamo 是编译时织入，所以运行时没有性能损失，和手写代码一样快。事务开销主要来自数据库，AOP 本身可以忽略不计。

## 总结

AntFlowCore 的事务处理设计非常简洁：

1. **Rougamo AOP** 提供编译时织入，零侵入
2. **[Transactional] 注解** 声明式事务，一句话开启事务
3. **FreeSql UnitOfWork** 提供核心事务能力
4. **传播级别** 完整支持，满足各种场景需求
5. **自动提交/回滚**：根据方法是否抛出异常自动决定，不需要手动调用

整个设计非常干净，没有引入复杂的框架，却提供了强大的声明式事务能力。

---

**相关链接：**
- [Rougamo 官方 GitHub](https://github.com/yuanboyu2018/Rougamo)
- [FreeSql UnitOfWork 文档](https://freesql.net/documentation/guide/unitofwork.html)
- [AntFlowCore 事件系统详解](./AntFlowCore-事件系统详解-监听流程状态变化.md)
- [上一篇：Natasha 低代码条件计算](./AntFlowCore-Natasha-低代码条件计算.md)
