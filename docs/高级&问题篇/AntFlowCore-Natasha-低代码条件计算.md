# AntFlowCore 中 Natasha 的妙用：动态实现 IAdaptor 工厂

## 前言

AntFlowCore .NET 版使用了 **Natasha** 动态编译框架，但很多人不知道，它主要不是用来做动态条件计算的，而是用来优雅解决 `IAdaptorFactory` 的动态实现问题。本文深入解析这个设计，看看 Natasha 怎么让代码变得更优雅。

## 问题背景：什么是 Adaptor

AntFlowCore 定义了 `IAdaptor` 接口，用来做各种适配：

```csharp
public interface IAdaptor
{
    // 适配接口
}

public interface IUserAdaptor : IAdaptor
{
    List<UserVo> QueryUsers(string keyword);
}

public interface IRoleAdaptor : IAdaptor
{
    List<RoleVo> QueryRoles();
}
```

设计思路：AntFlowCore 本身不需要关心用户系统是怎么实现的，不同项目用户系统不一样，所以定义接口，由使用者去适配。

问题来了：如果我有很多个 `IAdaptor` 接口，每个都需要用户自己实现，使用者要做很多工作，能不能简化？

## 常见的实现方式有什么问题

### 传统方式：每个接口手动实现

```csharp
public class MyUserAdaptor : IUserAdaptor
{
    public List<UserVo> QueryUsers(string keyword)
    {
        // 调用我的业务服务查询
        return _userService.Query(keyword);
    }
}

public class MyRoleAdaptor : IRoleAdaptor
{
    public List<RoleVo> QueryRoles()
    {
        // 调用我的业务服务查询
        return _roleService.QueryAll();
    }
}
```

每个接口都要写一个包装类，虽然不难，但是很繁琐，代码都是重复的，没什么技术含量，就是体力活。

### AntFlowCore 的需求：默认实现适配，如果用户没自定义就用默认，有自定义就用用户的

如果没有默认实现，每个使用者都要实现所有 Adaptor，太麻烦了。AntFlowCore 希望：

- 如果用户没有自定义实现，用默认的（从 AntFlowCore 自带的用户表查询）
- 如果用户自定义实现了，用用户的
- 不需要用户写一堆包装类，最好能自动适配

## Natasha 来救场：动态生成实现类

AntFlowCore 的设计思路：**利用 Natasha 动态编译，在运行时自动生成 `IAdaptor` 接口的实现类**。

实现原理：
1. 定义多个 `IXXXAdaptor` 接口
2. 用户如果自己实现了，就用用户的
3. 用户如果没实现，Natasha 动态生成一个默认实现类，编译加载，直接用
4. 使用者不需要写任何代码，只需要配置一下就行

这样既保持了可扩展性，又省去了使用者写一堆重复代码的麻烦。

## 核心源码解析

我们看核心代码 `antflowcore/factory/AdaptorFactoryBuilder.cs`:

```csharp
public static class AdaptorFactoryBuilder
{
    public static IAdaptor BuildDefaultAdaptor(Type adaptorInterface)
    {
        // 构建实现类代码
        var code = BuildAdaptorCode(adaptorInterface);
        
        // Natasha 动态编译
        var type = NatashaKit.CreateBuilder()
            .AddReference(adaptorInterface.Assembly)
            .WithCode(code)
            .ComplieAndLoadOneType();
        
        // 创建实例
        return (IAdaptor)Activator.CreateInstance(type);
    }

    private static string BuildAdaptorCode(Type adaptorInterface)
    {
        var namespaceName = "antflowcore.adaptor.dynamic";
        var className = "Dynamic" + adaptorInterface.Name;
        
        var methods = adaptorInterface.GetMethods()
            .Select(m => BuildMethod(m))
            .JoinToString("\n\n");
        
        return $@"
using System;
using System.Collections.Generic;
using antflowcore.entity;
using antflowcore.repository;
using antflowcore.vo;

namespace {namespaceName}
{{
    public class {className} : {adaptorInterface.FullName}
    {{
        private readonly IBaseRepository _repository;
        
        public {className}(IBaseRepository repository)
        {{
            _repository = repository;
        }}
        
        {methods}
    }}
}}
";
    }

    private static string BuildMethod(MethodInfo method)
    {
        // 根据方法签名自动生成方法体
        // 默认实现调用 AntFlowCore 自带仓储查询
        // ...
    }
}
```

就是这么神奇！Natasha 在运行时帮我们生成了实现类代码，编译加载，直接就能用，使用者完全感知不到。

## 工厂怎么选择实现

`IAdaptorFactory` 的选择逻辑：

```csharp
public class AdaptorFactory : IAdaptorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, IAdaptor> _defaultCache 
        = new ConcurrentDictionary<Type, IAdaptor>();

    public IAdaptor GetAdaptor(Type adaptorType)
    {
        // 先从 DI 找，用户如果已经注册了自定义实现，用用户的
        var adaptor = _serviceProvider.GetService(adaptorType) as IAdaptor;
        if (adaptor != null)
        {
            return adaptor;
        }

        // 没找到，用 Natasha 动态生成默认实现
        return _defaultCache.GetOrAdd(adaptorType, type => 
            AdaptorFactoryBuilder.BuildDefaultAdaptor(type));
    }
}
```

这个设计太优雅了：
1. **优先使用用户自定义实现** → 满足可扩展性
2. **用户没自定义，自动生成默认实现** → 省去用户写一堆重复代码
3. **缓存生成好的实现** → 不需要每次都编译，性能好

## 使用示例

使用者怎么用？太简单了：

如果你完全使用默认配置，不需要写任何代码，直接用：

```csharp
// 工厂自动给你返回 IUserAdaptor，如果没自定义，用动态生成的默认实现
var userAdaptor = _adaptorFactory.GetAdaptor<IUserAdaptor>();
var users = userAdaptor.QueryUsers("keyword");
```

如果你想自定义实现，只需要在 DI 注册你的实现：

```csharp
services.AddScoped<IUserAdaptor, MyCustomUserAdaptor>();
```

工厂会自动发现并使用你的自定义实现，不需要改其他任何代码。

## 优势对比

| 方式 | 使用者代码量 | 可扩展性 | 性能 |
|------|------------|---------|------|
| 每个接口手动实现 | 多（每个接口一个类） | 好 | 好 |
| 反射调用默认实现 | 少 | 好 | 反射比直接调用慢 |
| Natasha 动态生成实现 | 零（默认不需要写代码，自定义只需注册） | 好 | 编译一次后和原生调用一样快 |

Natasha 方案兼具了零代码默认配置和高性能，可扩展性也不受影响。

## 缓存策略

动态编译比较耗时，所以 AntFlowCore 对生成好的实现做了缓存：

```csharp
private readonly ConcurrentDictionary<Type, IAdaptor> _defaultCache 
    = new ConcurrentDictionary<Type, IAdaptor>();
```

同一个接口只需要编译一次，之后直接从缓存拿，性能完全没问题。

如果需要重新生成（比如应用重启），清空缓存就行，下次使用重新编译。

## 那条件计算呢？

AntFlowCore 确实也用 Natasha 做条件表达式编译，但是那不是主要用途，主要用途还是在这里：**解决 IAdaptor 工厂的动态实现问题，省去大量重复代码，让框架更易用**。

这样设计带来的好处：

- 对于新手：直接用默认配置就能跑起来，快速入门
- 对于生产环境：可以自定义各个 Adaptor，适配自己的业务系统
- 对于框架作者：不需要维护一堆默认实现类，代码更干净
- 双方都受益

## 为什么选择 Natasha 做这个

1. **API 简单**：Natasha 的 API 非常友好，几行代码就能完成动态编译生成类型
2. **性能好**：编译一次后缓存，和原生代码一样快
3. **内存管理友好**：支持卸载，不需要了可以释放
4. **社区活跃**：持续维护更新，比自己造轮子靠谱

## 常见问题

### 1. 如果不用 Natasha，还有其他方案吗？

有，可以用：
- **字典缓存委托**：需要你手动去做，比较繁琐
- **Castle 动态代理**：可以做，但比 Natasha 重，而且主要是做AOP，生成实现类不如 Natasha 直接
- **源生成器**：.NET 6+ 源生成器可以做，但需要编译时知道类型，这里是运行时决定，不适用

Natasha 是目前来说最合适的方案。

### 2. 这个设计有什么缺点吗？

- 首次使用需要编译，会有一点点延迟，但是只发生一次，之后就好了
- 需要 Natasha 依赖，但是 Natasha 本身非常轻量，不是问题

### 3. 我完全自定义所有 Adaptor，还需要 Natasha 吗？

需要，就算你全部自定义，DI 能找到你的实现，根本不会走到动态生成那一步，所以 Natasha 不会影响你，也不会有额外开销。

### 4. 编译失败了怎么办？

理论上不会，因为代码是自动生成的，模板没问题，生成出来肯定能编译过。如果真编译失败了，检查一下你的接口定义是否符合规范。

### 5. 支持异步方法吗？

完全支持，自动生成代码的时候会识别方法返回类型，如果是 `Task<T>`，会正确生成异步方法体。

## 总结

AntFlowCore 用 Natasha 解决了一个非常实际的问题：如何既提供默认实现，又保持良好的可扩展性，同时还不让使用者写太多重复代码。

通过 Natasha 动态生成实现类：

- ✅ 使用者默认零代码，就能跑起来
- ✅ 需要自定义时，只需要注册你的实现，不需要改其他代码
- ✅ 编译一次缓存起来，性能和原生一样
- ✅ 框架代码更干净，不需要维护一堆重复的实现类

这就是 Natasha 在 AntFlowCore 中的真正妙用，是不是很优雅？

---

**相关链接：**
- [Natasha 官方 GitHub](https://github.com/dotnetcore/Natasha)
- [AntFlowCore 虚拟节点(VNode)模式深度解析](./AntFlowCore-虚拟节点-VNode-模式深度解析.md)
- [AntFlowCore 事务处理详解](./AntFlowCore-事务处理详解-AOP-UnitOfWork.md)
