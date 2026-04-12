# AntFlowCore 审批人查找策略详解：从直接指定到组织架构动态查找

## 前言

流程审批中，最核心的问题之一就是：**当前这个节点，到底应该由谁来审批？**

AntFlowCore 内置了十几种审批人查找策略，满足各种场景需求，从简单的直接指定，到复杂的多级上级动态查找，都能轻松应对。本文详细介绍各种策略的用法和实现原理。

## 核心接口：IBpmnPersonnelProviderService

所有审批人查找策略都实现这个接口：

```csharp
public interface IBpmnPersonnelProviderService
{
    /// <summary>
    /// 判断当前策略是否支持这个配置
    /// </summary>
    bool Support(BpmnNodePersonnelConf conf);
    
    /// <summary>
    /// 查找审批人列表
    /// </summary>
    Task<List<string>> QueryUsers(BpmnNodePersonnelConf conf, AFProcessInstance instance);
}
```

AntFlowCore 使用工厂模式管理所有策略，根据配置自动选择对应的策略，你也可以轻松自定义扩展。

## 内置的审批人查找策略

AntFlowCore 内置了以下常见策略：

| 策略 | 适用场景 |
|------|---------|
| 用户指定 | 直接指定固定用户 |
| 角色指定 | 根据角色查找，角色中所有用户都是审批人 |
| 发起人自己 | 发起人就是审批人 |
| 部门负责人 | 发起人所在部门负责人 |
| 直属上级 | 发起人的直属上级 |
| 多级上级 | 一直往上找 N 级上级 |
| HRBP | 对应部门的 HRBP |
| 循环审批 | 循环找每个部门负责人（比如分公司→集团） |
| 自定义业务表 | 从你自己的业务表中查找 |
| 外部指定 | 外部流程调用时传入 |

我们一个一个来看。

## 1. 直接指定用户

最简单也最常用：在流程设计器中直接选好人，固定就是这些人审批。

**适用场景**：固定岗位，比如总经理审批，不会变。

**实现源码**：`UserPointedPersonnelProvider.cs`

```csharp
public class UserPointedPersonnelProvider : IBpmnPersonnelProviderService
{
    public bool Support(BpmnNodePersonnelConf conf)
    {
        return conf.PointedUserList != null && conf.PointedUserList.Any();
    }

    public Task<List<string>> QueryUsers(BpmnNodePersonnelConf conf, AFProcessInstance instance)
    {
        // 直接返回配置好的用户ID列表
        var users = conf.PointedUserList
            .Select(x => x.UserId)
            .ToList();
        return Task.FromResult(users);
    }
}
```

就是这么简单，直接返回配置好的用户列表。

## 2. 按角色指定

配置的时候选择角色，运行时动态查找这个角色下所有用户。

**适用场景**：比如"部门经理"角色，任何部门经理都在这个角色里，运行时自动查找。

**实现**：`RolePersonnelProvider.cs`

```csharp
public class RolePersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IRoleQueryService _roleQueryService;

    public bool Support(BpmnNodePersonnelConf conf)
    {
        return !string.IsNullOrEmpty(conf.RoleId);
    }

    public async Task<List<string>> QueryUsers(BpmnNodePersonnelConf conf, AFProcessInstance instance)
    {
        // 根据角色ID查询所有用户
        var users = await _roleQueryService.QueryUsersByRoleId(conf.RoleId);
        return users.Select(x => x.id).ToList();
    }
}
```

## 3. 发起人自己

审批人就是流程发起人自己。

**适用场景**：发起人填写表单，需要自己确认提交。

**实现**：`StartUserPersonnelProvider.cs`

```csharp
public class StartUserPersonnelProvider : IBpmnPersonnelProviderService
{
    public bool Support(BpmnNodePersonnelConf conf)
    {
        return true; // 配置选了这个策略就支持
    }

    public Task<List<string>> QueryUsers(BpmnNodePersonnelConf conf, AFProcessInstance instance)
    {
        // 流程发起人就是审批人
        return Task.FromResult(new List<string> { instance.StartUserId });
    }
}
```

## 4. 部门负责人

发起人所在部门的负责人。

**适用场景**：任何发起人，都由他自己部门负责人审批，不需要配置死。

**实现**：`LevelPersonnelProvider.cs`（层级模式的一种）

核心逻辑：
1. 从流程变量或发起人信息拿到发起人部门ID
2. 查询这个部门的负责人
3. 返回负责人用户ID

如果你的组织架构存在多级部门，可以配置层级查找。

## 5. 直属上级

找到发起人的直接上级。

**适用场景**：一般请假流程，第一步都是直属上级审批。

**实现**：`DirectLeaderPersonnelProvider.cs`

```csharp
public class DirectLeaderPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly IEmployeeService _employeeService;

    public async Task<List<string>> QueryUsers(
        BpmnNodePersonnelConf conf, 
        AFProcessInstance instance)
    {
        // 获取发起人ID
        var startUserId = instance.StartUserId;
        // 查询发起人的直属上级ID
        var leaderId = await _employeeService.GetDirectLeader(startUserId);
        return new List<string> { leaderId };
    }
}
```

非常直接，就是找到发起人上级。

## 6. 多级上级

一直往上找 N 级上级。比如找直属上级的上级，一直到第 N 级。

**适用场景**：金额越大，需要越高层级审批，比如：
- 小于3天：直属上级审批
- 大于3天：直属上级 → 部门经理 两级审批

**配置**：在流程设计器中配置级数。

**实现**：

```csharp
public class LevelPersonnelProvider : IBpmnPersonnelProviderService
{
    public async Task<List<string>> QueryUsers(
        BpmnNodePersonnelConf conf, 
        AFProcessInstance instance)
    {
        var result = new List<string>();
        var currentUserId = instance.StartUserId;
        var level = conf.Level ?? 1; // 需要找几级
        
        for (int i = 0; i < level; i++)
        {
            var leader = await _employeeService.GetDirectLeader(currentUserId);
            if (string.IsNullOrEmpty(leader))
            {
                break;
            }
            result.Add(leader);
            currentUserId = leader;
        }
        
        return result;
    }
}
```

这样就会依次找到每一级上级，每一级一个审批任务。

## 7. 自定义业务表查找

最灵活的方式：从你自己的业务表中根据条件查找审批人。

**适用场景**：你的组织架构存在你的业务表中，有特殊的查找规则。

**实现**：`BusinessTablePersonnelProvider.cs`

你只需要在流程设计中配置：
- 表名
- 条件字段
- 条件值（可以从流程变量取）
- 结果用户名字段

AntFlowCore 会自动帮你查询：

```csharp
public async Task<List<string>> QueryUsers(
    BpmnNodePersonnelConf conf, 
    AFProcessInstance instance)
{
    // 从配置中拿到表名、条件字段、结果字段
    var tableName = conf.BusinessTable;
    var conditionColumn = conf.BusinessTableConditionColumn;
    var conditionValue = instance.Variable.GetValue<string>(conf.BusinessTableConditionValue);
    var resultColumn = conf.BusinessTableResultColumn;
    
    // 自由Sql查询，返回用户ID列表
    var users = await _freeSql.Select<Object>(tableName)
        .Where($"{conditionColumn} = @value", new { value = conditionValue })
        .ToList<string>(resultColumn);
    
    return users;
}
```

这种方式非常灵活，不需要写代码，只需要在设计器配置就能搞定。

## 8. 外部传入

启动流程的时候，把审批人传进来，运行时直接用传入的。

**适用场景**：外部系统发起流程，提前知道审批人是谁。

**实现**：`OutSidePersonnelProvider.cs`

```csharp
public async Task<List<string>> QueryUsers(
    BpmnNodePersonnelConf conf, 
    AFProcessInstance instance)
{
    // 从流程变量中获取外部传入的审批人列表
    var userIds = instance.Variable.GetValue<List<string>>("assignees");
    return userIds ?? new List<string>();
}
```

启动流程的时候，把审批人放在流程变量里就行：

```csharp
await _processStarter.StartProcess(
    "leave",
    "business_123",
    new Dictionary<string, object>
    {
        { "assignees", new List<string> { "user1", "user2" } }
    });
```

## 如何扩展自定义审批人策略

如果内置策略满足不了你的需求，扩展非常简单，只需要三步：

### 步骤一：实现接口

```csharp
public class MyCustomPersonnelProvider : IBpmnPersonnelProviderService
{
    public bool Support(BpmnNodePersonnelConf conf)
    {
        // 判断是否支持这个配置，你可以自定义一个标记
        return !string.IsNullOrEmpty(conf.MyCustomConfig);
    }

    public async Task<List<string>> QueryUsers(
        BpmnNodePersonnelConf conf, 
        AFProcessInstance instance)
    {
        // 你的自定义查找逻辑
        var result = await MyQueryUsers(conf, instance);
        return result;
    }

    private Task<List<string>> MyQueryUsers(
        BpmnNodePersonnelConf conf, 
        AFProcessInstance instance)
    {
        // 你的业务逻辑，比如调用你组织架构服务查找
        throw new NotImplementedException();
    }
}
```

### 步骤二：注册到 DI

```csharp
services.AddSingleton<IBpmnPersonnelProviderService, MyCustomPersonnelProvider>();
```

### 步骤三：流程设计器中使用

在流程设计器配置节点的时候，选择你的自定义策略，配置相关参数就行了。

就是这么简单，不需要改 AntFlowCore 任何代码。

## 源码解析：工厂如何选择策略

我们来看 `BpmnPersonnelProviderFactory` 是怎么工作的：

```csharp
public class BpmnPersonnelProviderFactory
{
    private readonly IEnumerable<IBpmnPersonnelProviderService> _providers;

    public BpmnPersonnelProviderFactory(IEnumerable<IBpmnPersonnelProviderService> providers)
    {
        _providers = providers;
    }

    public IBpmnPersonnelProviderService GetProvider(BpmnNodePersonnelConf conf)
    {
        var provider = _providers.FirstOrDefault(p => p.Support(conf));
        if (provider == null)
        {
            throw new NotSupportedException(
                $"找不到对应的审批人查找策略，配置类型：{conf.Type}");
        }
        return provider;
    }
}
```

利用 DI 自动注入所有实现，然后根据 `Support` 方法判断哪个支持当前配置，返回对应的提供者。这就是开闭原则：新增策略只需要加新实现，不需要改工厂代码。

## 最佳实践

### 1. 简单场景用直接指定

如果审批人固定不变，直接指定最简单，性能最好。

### 2. 按角色分工用角色指定

如果是按岗位分工，岗位上的人会变，用角色指定，运行时动态查找，不需要改流程配置。

### 3. 基于组织架构用动态查找

直属上级、部门负责人、多级上级这些策略，就是为基于组织架构的动态查找设计的，满足绝大多数企业审批场景。

### 4. 特殊需求用自定义业务表

如果你有特殊的查找规则，不需要改代码，用自定义业务表配置就能搞定，非常灵活。

## 常见问题

### 1. 查找到多个审批人，怎么处理？

返回多个审批人ID，AntFlowCore 会给每个审批人创建一个任务，根据你的会签配置处理（会签还是顺序审批）。

### 2. 查找结果为空怎么办？

如果查找结果为空，AntFlowCore 会走配置的"审批人不存在"处理策略（跳过或者转交给管理员）。

### 3. 需要加上发起人自己吗？

不需要，你返回列表里加上就行，不加就没有，完全由你控制。

### 4. 可以多个策略组合吗？

可以，比如你同时配置直接指定 + 角色指定，两个结果会合并。

### 5. 支持异步查找吗？

支持，接口就是 `async Task`，你可以异步调用你的组织架构服务。

## 总结

AntFlowCore 审批人查找设计非常清晰：

- **接口抽象**：`IBpmnPersonnelProviderService` 定义统一行为
-** 开闭原则 **：新增策略不修改现有代码，只需要加新实现
- **丰富内置策略**：覆盖绝大多数企业场景，开箱即用
- **灵活扩展**：满足不了需求可以自己扩展，非常简单

不管你的审批人规则有多复杂，AntFlowCore 都能搞定。

---

**相关链接：**
- [AntFlowCore 虚拟节点(VNode)模式深度解析](./AntFlowCore-虚拟节点-VNode-模式深度解析.md)
- [AntFlowCore 自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
- [上一篇：高级技巧：使用 Natasha 动态生成工厂实现](./AntFlowCore-高级技巧-使用-Natasha-动态生成工厂实现.md)
