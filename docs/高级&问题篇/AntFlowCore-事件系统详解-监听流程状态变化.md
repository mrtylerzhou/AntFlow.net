# AntFlowCore 事件系统详解 - 如何监听流程状态变化

## 前言

在实际流程开发中，我们经常需要在流程状态变化时做一些业务处理：比如审批通过后更新业务单据状态、发送通知消息、触发下游业务等等。AntFlowCore 提供了完整的事件系统，让你可以轻松监听各种流程事件，做出自定义处理。

本文详细介绍 AntFlowCore 事件系统的设计原理和使用方法。

## 事件系统设计思路

AntFlowCore 采用**领域事件**模式，在流程发生状态变化时，会发布对应的事件，你只需要订阅你感兴趣的事件，就能在事件发生时收到通知。

这种设计的好处：
- 松耦合：AntFlow 核心和你的业务代码完全解耦
- 灵活：你可以订阅任意事件，做任意处理
- 可扩展：新增事件不影响核心代码

## 有哪些事件可以监听

AntFlowCore 定义了以下几类事件：

| 事件类型 | 触发时机 |
|---------|---------|
| `ProcessStartedEvent` | 流程实例创建并启动完成 |
| `TaskCreatedEvent` | 任务创建完成（新任务到达） |
| `TaskCompletedEvent` | 任务完成（审批人处理完成） |
| `TaskClaimedEvent` | 任务被签收（对于候选组任务） |
| `ProcessCompletedEvent` | 流程实例结束完成 |
| `ProcessRejectedEvent` | 流程被驳回 |
| `ProcessWithdrawnEvent` | 流程被撤回 |
| `CustomButtonClickedEvent` | 自定义按钮点击后 |

所有事件都继承自 `BaseFlowEvent`。

## 两种订阅方式

AntFlowCore 支持两种订阅方式，你可以任选一种：

### 方式一：实现 `IFlowEventHandler<TEvent>` 接口（推荐）

这是最简洁的方式，一个处理器处理一种事件。

```csharp
public interface IFlowEventHandler<TEvent> where TEvent : BaseFlowEvent
{
    Task HandleAsync(TEvent @event);
}
```

### 方式二：使用 `[EventHandler]` 特性标记方法

这种适合把多个事件处理放在一个类里。

```csharp
public class MyFlowEventHandlers
{
    [EventHandler]
    public async Task OnProcessStarted(ProcessStartedEvent @event)
    {
        // 处理逻辑
    }

    [EventHandler]
    public async Task OnProcessCompleted(ProcessCompletedEvent @event)
    {
        // 处理逻辑
    }
}
```

两种方式都可以，AntFlowCore 会自动扫描发现所有处理器。

## 自动注册原理

和自定义按钮一样，AntFlowCore 自动扫描发现所有事件处理器，不需要手动注册。

源码 `antflowcore/conf/di/AntFlowServiceSetup.cs`:

```csharp
// 扫描 IFlowEventHandler<>
var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => !t.IsAbstract)
    .ToList();

foreach (var type in handlerTypes)
{
    // 检查是否实现了 IFlowEventHandler<>
    var interfaces = type.GetInterfaces()
        .Where(i => i.IsGenericType && 
               i.GetGenericTypeDefinition() == typeof(IFlowEventHandler<>))
        .ToList();

    foreach (var intf in interfaces)
    {
        services.AddScoped(intf, type);
        eventBus.RegisterHandler(intf.GetGenericArguments()[0], type);
    }

    // 检查 [EventHandler] 特性标记的方法
    var methods = type.GetMethods()
        .Where(m => m.GetCustomAttributes(typeof(EventHandlerAttribute), false).Any())
        .ToList();

    foreach (var method in methods)
    {
        // ... 注册方法处理器
    }
}
```

和自定义按钮一样，**约定大于配置**，写完就自动生效了。

## 实战示例：审批通过后发送通知并更新业务状态

我们来看一个最常用的场景：流程结束后，更新业务单据状态，发送通知给申请人。

```csharp
public class ProcessCompletedEventHandler : IFlowEventHandler<ProcessCompletedEvent>
{
    private readonly IBusinessOrderService _businessOrderService;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public ProcessCompletedEventHandler(
        IBusinessOrderService businessOrderService,
        INotificationService notificationService,
        IUserService userService)
    {
        _businessOrderService = businessOrderService;
        _notificationService = notificationService;
        _userService = userService;
    }

    public async Task HandleAsync(ProcessCompletedEvent @event)
    {
        // 1. 从事件获取流程实例
        var instance = @event.ProcessInstance;
        
        // 2. BusinessKey 就是你发起流程时传入的业务单据ID
        var businessKey = instance.BusinessKey;
        
        if (string.IsNullOrWhiteSpace(businessKey))
        {
            return;
        }

        // 3. 更新业务单据状态为已审批
        await _businessOrderService.UpdateStatusAsync(businessKey, "Approved");

        // 4. 获取申请人信息
        var startUserId = instance.StartUserId;
        var startUser = await _userService.GetUserByIdAsync(long.Parse(startUserId));

        // 5. 发送通知给申请人：你的请假申请已审批完成
        await _notificationService.SendNotificationAsync(
            startUserId: startUserId,
            title: "审批完成",
            content: $"你的请假申请已审批通过");
    }
}
```

完成了！就是这么简单。不需要配置，不需要注册，写完就生效。

## 另一个例子：任务创建后通知审批人

当新任务到达时，通知审批人有待办需要处理：

```csharp
public class TaskCreatedEventHandler : IFlowEventHandler<TaskCreatedEvent>
{
    private readonly INotificationService _notificationService;

    public TaskCreatedEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(TaskCreatedEvent @event)
    {
        var task = @event.Task;
        var instance = @event.ProcessInstance;

        // 获取审批人
        var assignee = task.Assignee;
        if (string.IsNullOrWhiteSpace(assignee))
        {
            return;
        }

        // 发送通知：你有新的待办任务
        await _notificationService.SendEmailAsync(
            to: assignee,
            subject: $"你有新的待办任务：{instance.ProcessDefinitionName}",
            body: $"流程名称：{instance.ProcessDefinitionName}\n请及时处理");
    }
}
```

以后每创建一个新任务，都会自动发邮件通知审批人。

## 使用特性方式订阅

如果你喜欢把多个事件处理放在一个类里，可以用特性方式：

```csharp
public class AntFlowNotificationHandlers
{
    private readonly INotificationService _notificationService;
    private readonly IBusinessOrderService _businessOrderService;

    public AntFlowNotificationHandlers(
        INotificationService notificationService,
        IBusinessOrderService businessOrderService)
    {
        _notificationService = notificationService;
        _businessOrderService = businessOrderService;
    }

    [EventHandler]
    public async Task OnTaskCreated(TaskCreatedEvent @event)
    {
        // 通知审批人
        var assignee = @event.Task.Assignee;
        if (!string.IsNullOrEmpty(assignee))
        {
            await _notificationService.NotifyNewTask(assignee, @event.ProcessInstance);
        }
    }

    [EventHandler]
    public async Task OnProcessCompleted(ProcessCompletedEvent @event)
    {
        // 更新业务状态
        var businessKey = @event.ProcessInstance.BusinessKey;
        if (!string.IsNullOrEmpty(businessKey))
        {
            await _businessOrderService.UpdateStatus(businessKey, "Approved");
            await _notificationService.NotifyProcessCompleted(
                @event.ProcessInstance.StartUserId, 
                @event.ProcessInstance);
        }
    }

    [EventHandler]
    public async Task OnProcessRejected(ProcessRejectedEvent @event)
    {
        // 通知申请人被驳回
        var businessKey = @event.ProcessInstance.BusinessKey;
        await _businessOrderService.UpdateStatus(businessKey, "Rejected");
        await _notificationService.NotifyRejected(
            @event.ProcessInstance.StartUserId, 
            @event.ProcessInstance);
    }
}
```

这种方式适合把相关的事件处理组织在一起，也非常清晰。

## 获取流程变量

事件中可以直接获取流程变量，方便你做业务处理：

```csharp
public async Task HandleAsync(ProcessStartedEvent @event)
{
    var instance = @event.ProcessInstance;
    
    // 获取流程变量
    var applicantName = instance.Variable.GetValue<string>("applicantName");
    var leaveDays = instance.Variable.GetValue<decimal>("leaveDays");
    var department = instance.Variable.GetValue<string>("department");

    // 用这些变量做业务处理...
}
```

`instance.Variable` 就是你发起流程时传入的变量，直接用就行。

## 异步事件和同步事件

AntFlowCore 默认是异步处理事件，事件发布后，事件处理在后台执行，不会阻塞主流程。

如果你的业务需要事件处理和主流程同一个事务（也就是事件处理失败，主流程也要回滚），可以在注册时指定同步模式。不过大多数场景下，异步足够了。

## 异常处理

如果事件处理抛出异常，会怎么样？

- 默认情况下，异常会被记录日志，但不会影响主流程继续执行
- 如果你希望事件处理失败时，主流程也回滚，可以让异常继续抛出，事务会自动回滚

示例：希望验证失败时阻止流程继续：

```csharp
public async Task HandleAsync(ProcessStartedEvent @event)
{
    var instance = @event.ProcessInstance;
    var businessKey = instance.BusinessKey;
    
    var order = await _orderService.GetByIdAsync(businessKey);
    if (order == null)
    {
        // 业务单据不存在，抛出异常，阻止流程启动
        throw new BusinessException($"业务单据 {businessKey} 不存在，无法启动流程");
    }

    // ...
}
```

这样如果业务单据不存在，流程启动会失败，回滚所有操作，符合预期。

## 事件调用顺序

整个流程处理中，事件触发顺序：

```
启动流程
  ↓
发布 ProcessStartedEvent
  ↓
走到第一个节点
  ↓
检查条件
  ↓
查找审批人
  ↓
创建任务
  ↓
发布 TaskCreatedEvent
  ↓
↓↓↓ 等待审批 ↓↓↓
  ↓
审批人处理完成
  ↓
发布 TaskCompletedEvent
  ↓
推进到下一个节点
  ↓
...重复...
  ↓
流程结束
  ↓
发布 ProcessCompletedEvent
  ↓
结束
```

你可以根据你的需求在合适的阶段做处理。

## 实战：实现一个事件日志记录器

我们来做一个更实用的例子：记录所有流程事件到数据库，方便审计排查。

```csharp
public class EventLoggingHandler<TEvent> : IFlowEventHandler<TEvent> 
    where TEvent : BaseFlowEvent
{
    private readonly IFlowEventLogService _eventLogService;

    public EventLoggingHandler(IFlowEventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    public async Task HandleAsync(TEvent @event)
    {
        await _eventLogService.InsertAsync(new FlowEventLog
        {
            EventName = typeof(TEvent).Name,
            ProcessInstanceId = @event.ProcessInstance.Id,
            TaskId = @event is BaseTaskEvent taskEvent ? taskEvent.Task.Id : null,
            EventTime = DateTime.Now,
            EventData = Serialize(@event)
        });
    }
}
```

然后利用 C# 泛型，你可以一口气注册所有事件类型：

```csharp
// 当然，你也可以每个事件写一个，这里只是展示技巧
builder.Services.AddScoped<IFlowEventHandler<ProcessStartedEvent>, EventLoggingHandler<ProcessStartedEvent>>();
builder.Services.AddScoped<IFlowEventHandler<TaskCreatedEvent>, EventLoggingHandler<TaskCreatedEvent>>();
builder.Services.AddScoped<IFlowEventHandler<TaskCompletedEvent>, EventLoggingHandler<TaskCompletedEvent>>();
builder.Services.AddScoped<IFlowEventHandler<ProcessCompletedEvent>, EventLoggingHandler<ProcessCompletedEvent>>();
```

这样所有事件都会被自动记录下来。

## 常见问题

### 1. 我可以订阅同一个事件多个处理器吗？

完全可以！AntFlowCore 会按顺序调用所有订阅了该事件的处理器。

比如：
- 一个处理器更新业务状态
- 一个处理器发送通知
- 一个处理器记录日志

三个处理器分开，各司其职，符合单一职责原则。

### 2. 事件会丢失吗？

在当前事务模型下，如果主流程提交成功，事件一定会发布。如果主流程回滚，事件不会发布，符合预期。

### 3. 可以异步处理吗？会影响性能吗？

默认就是异步处理，不会阻塞主流程，对性能影响很小。如果你的事件处理比较耗时（比如调用外部 API 发送邮件），建议放在事件处理中，这样主流程不会被拖慢。

### 4. 我需要集成到第三方系统，比如钉钉/企业微信通知，应该放在哪里？

就放在对应的事件处理中，比如：
- `TaskCreatedEvent` 处理中发钉钉消息给审批人
- `ProcessCompletedEvent` 处理中发企业微信通知给申请人

非常清晰。

### 5. 我的业务逻辑需要修改流程变量，在哪里改？

可以在事件处理中直接修改 `instance.Variable`，然后保存：

```csharp
public async Task HandleAsync(ProcessStartedEvent @event)
{
    var instance = @event.ProcessInstance;
    // 添加额外的流程变量
    instance.Variable.Set("extraInfo", "这是额外信息");
    // 保存
    await _processInstanceService.UpdateInstanceAsync(instance);
}
```

### 6. 支持分布式事件吗？发到消息队列那种。

AntFlowCore 核心不直接提供，但是你可以很容易自己实现：

```csharp
public class MyHandler : IFlowEventHandler<ProcessCompletedEvent>
{
    private readonly IMessageQueue _messageQueue;

    public async Task HandleAsync(ProcessCompletedEvent @event)
    {
        // 发布到消息队列，让下游服务消费
        await _messageQueue.PublishAsync(
            "flow.process.completed", 
            new { @event.ProcessInstance.Id, @event.ProcessInstance.BusinessKey });
    }
}
```

非常简单。

## 总结

AntFlowCore 的事件系统设计非常简洁而强大：

- **多种订阅方式**：接口方式或特性方式，任选
- **自动发现注册**：写完就生效，不需要手动配置
- **松耦合**：你的业务代码不依赖 AntFlow 核心太多，只需要订阅事件
- **灵活**：一个事件可以有多个处理器，随时增减不影响其他代码

通过事件系统，你可以非常方便地在流程的各个生命周期节点插入你的业务逻辑，满足各种个性化需求。

---

**相关链接：**
- [AntFlowCore 自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
- [AntFlowCore 多租户实现原理与实战](./AntFlowCore-多租户实现原理与实战.md)
- [上一篇：自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
