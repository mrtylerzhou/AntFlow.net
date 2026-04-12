# AntFlowCore 高级技巧：使用 Natasha 动态生成工厂实现

## 前言

AntFlowCore 大量使用了 Natasha 动态编译能力来简化代码，其中最巧妙的一个用法就是**动态生成 `IAdaptorFactory` 工厂实现**。本文深入解析这个设计，看看它是如何省去我们手写大量工厂代码的。

## 问题背景：工厂模式的麻烦

我们先来看传统工厂模式的写法。假设你有一个接口 `IAdaptorFactory`，里面有多个方法，每个方法根据参数返回不同的实现：

```csharp
public interface IAdaptorFactory
{
    IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum);
    
    IFormOperationAdaptor<BusinessDataVo> GetActivitiService(BusinessDataVo dataVo);
    
    IProcessOperationAdaptor GetProcessOperation(BusinessDataVo vo);
    
    AbstractBusinessConfigurationAdaptor GetBusinessConfigurationAdaptor(ConfigurationTableAdapterEnum byTableFieldEnum);
    
    BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
}
```

如果手写实现，你会写出这样的代码：

```csharp
public class AdaptorFactory : IAdaptorFactory
{
    public IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum)
    {
        switch(adpConfEnum)
        {
            case BpmnNodeAdpConfEnum.Type1:
                return new Type1BpmnNodeAdaptor();
            case BpmnNodeAdpConfEnum.Type2:
                return new Type2BpmnNodeAdaptor();
            // ... 每个枚举都要写一个 case
            default:
                throw new NotSupportedException();
        }
    }

    public IFormOperationAdaptor<BusinessDataVo> GetActivitiService(BusinessDataVo dataVo)
    {
        // 又一堆 switch case ...
    }

    // ... 每个方法都要手写一堆 switch case
}
```

这种写法问题很明显：
- **每次新增实现，都要改工厂代码**，违反开闭原则
- **代码重复，大量样板代码**，没技术含量，就是体力活
- **容易出错**，漏写一个 case 运行时才报错

AntFlowCore 是怎么解决这个问题的？**让 Natasha 帮我们动态生成工厂代码！**

## 设计思路：特性标记 + 动态生成

AntFlowCore 的设计非常优雅：

1. 在 `IAdaptorFactory` 接口的方法上标记特性
   - `[SpfService]`：指定对应的解析器类型
   - `[AutoParse]`：自动根据参数类型解析
2. 启动时，Natasha 读取接口方法信息，动态生成完整的工厂实现类
3. 编译加载，直接实例化使用
4. 新增实现只需要加特性，不需要改工厂代码

完全自动，零手工代码！

## 核心源码解析

我们来看核心代码 `AdaptorFactoryProxy.cs`:

### 1. 获取接口，生成代码

```csharp
// 获取接口类型
var interfaceType = Type.GetType($"antflowcore.factory.{SIMPLECLSNAME}");
if (interfaceType == null)
{
    throw new InvalidOperationException($"Cannot find interface {SIMPLECLSNAME}");
}

// 生成代理类代码
var proxyClassCode = GenerateProxyClass(interfaceType);

// Natasha 编译
var assembly = new AssemblyCSharpBuilder()
                .Add(proxyClassCode)
                .GetAssembly();

// 获取类型，实例化
var proxyType = assembly.GetType($"{PROXYFREFIX}{SIMPLECLSNAME}{PROXYSUFFIX}");
loadedInstance = Activator.CreateInstance(proxyType);
return (IAdaptorFactory)loadedInstance;
```

就是这么直接！读取接口信息，生成代码，编译，实例化，搞定。

### 2. 生成方法代码

我们来看 `GenerateProxyClass` 方法怎么生成一个方法：

```csharp
foreach (var method in methods)
{
    var methodName = method.Name;
    var returnType = method.ReturnType;
    var parameters = method.GetParameters();

    // 读取方法上的特性
    SpfServiceAttribute? spfServiceAttribute = 
        method.GetCustomAttribute<SpfServiceAttribute>();
    Type tagParserType = null;
    
    if (spfServiceAttribute != null)
    {
        tagParserType = spfServiceAttribute.TagParser;
    }
    else
    {
        AutoParseAttribute? autoParseAttribute = 
            method.GetCustomAttribute<AutoParseAttribute>();
        if (autoParseAttribute == null)
        {
            throw new AFBizException(
                $"method:{methodName} has neither TagParser nor a AutoParseAttribute");
        }
        // AutoParse 也动态生成
        object proxyInstance = 
            AutoParseProxyFactory.GetProxyInstance(parameters, returnType);
        tagParserType = proxyInstance.GetType();
    }

    // 生成方法签名
    sb.AppendLine($"public {returnTypeName} {methodName}(" +
        $"{string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");
    
    // 生成方法体：直接实例化解析器，调用 ParseTag
    sb.AppendLine("{");
    sb.AppendLine($" dynamic instance = " +
        $"System.Activator.CreateInstance(typeof({tagParserType.FullName}));");
    sb.AppendLine($" return instance.ParseTag({parameterDeclaration});");
    sb.AppendLine("}");
}
```

太漂亮了！这段代码做的事情：

1. 检查方法上有没有 `[SpfService]` 或 `[AutoParse]` 特性，没有就直接抛异常提示你
2. 根据特性拿到对应的解析器类型
3. 生成方法代码，方法体内直接 `Activator.CreateInstance` 然后调用 `ParseTag`
4. 全部自动生成，不需要你写一行代码

### 3. 生成出来的代码长什么样

我们来看看动态生成出来的代码大概是什么样：

```csharp
public class ProxyIAdaptorFactoryImpl : IAdaptorFactory
{
    public IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum)
    {
        dynamic instance = Activator.CreateInstance(typeof(BpmnNodeAdaptorTagParser));
        return instance.ParseTag(adpConfEnum);
    }

    public IFormOperationAdaptor<BusinessDataVo> GetActivitiService(BusinessDataVo dataVo)
    {
        dynamic instance = Activator.CreateInstance(typeof(ActivitiTagParser<>));
        return instance.ParseTag(dataVo);
    }

    public IProcessOperationAdaptor GetProcessOperation(BusinessDataVo vo)
    {
        dynamic instance = Activator.CreateInstance(typeof(FormOperationTagParser));
        return instance.ParseTag(vo);
    }

    public AbstractBusinessConfigurationAdaptor GetBusinessConfigurationAdaptor(
        ConfigurationTableAdapterEnum byTableFieldEnum)
    {
        dynamic instance = Activator.CreateInstance(typeof(BusinessConfigurationAdaptorTagParser));
        return instance.ParseTag(byTableFieldEnum);
    }

    public BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum)
    {
        dynamic instance = Activator.CreateInstance(typeof(BpmnElementAdaptorTagParser));
        return instance.ParseTag(nodePropertyEnum);
    }
}
```

就是这样！完全就是你手写的代码，但是它是自动生成的，你不需要动手写。

## 两种特性用法

### 1. `[SpfService]` 直接指定解析器

用法：

```csharp
public interface IAdaptorFactory
{
    [SpfService(typeof(FormOperationTagParser))]
    IProcessOperationAdaptor GetProcessOperation(BusinessDataVo vo);
}
```

简单直接，你直接告诉它用哪个解析器，生成代码直接实例化这个解析器。

### 2. `[AutoParse]` 自动解析

更高级的用法，自动根据返回类型解析：

```csharp
public interface IAdaptorFactory
{
    [AutoParse]
    IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum adpConfEnum);
    
    [AutoParse]
    BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
}
```

`AutoParseProxyFactory` 会自动根据枚举参数找到对应的实现，帮你生成解析器代码，你连 `[SpfService]` 都不用写。

## 使用方式

在你的项目中怎么用？太简单了：

```csharp
// 启动时获取代理实例，注册到 DI
IAdaptorFactory adaptorFactory = AdaptorFactoryProxy.GetProxyInstance();
services.AddSingleton(adaptorFactory);
```

然后直接注入使用：

```csharp
public class MyService
{
    private readonly IAdaptorFactory _adaptorFactory;

    public MyService(IAdaptorFactory adaptorFactory)
    {
        _adaptorFactory = adaptorFactory;
    }

    public void SomeMethod(BpmnNodeAdpConfEnum type)
    {
        var adaptor = _adaptorFactory.GetBpmnNodeAdaptor(type);
        // 使用 adaptor
    }
}
```

就是正常用，完全感知不到这是动态生成的。

## 优势对比

| 方式 | 代码量 | 开闭原则 | 可读性 | 性能 |
|------|--------|---------|--------|------|
| 手写 switch case | 多，每个方法一堆case | 不符合，新增要改 | 差，一大坨 | 好 |
| 字典缓存 | 中，需要手工填充字典 | 不符合，新增要改 | 一般 | 好 |
| Natasha 动态生成 | 零，完全自动 | 符合，新增只需要加特性 | 好，接口就是文档 | 编译一次后和手写一样快 |

## 性能问题

- **编译只发生一次**：启动的时候编译一次，生成好就缓存了，运行时不会重复编译
- **运行时就是直接调用**：和手写代码一样，没有反射开销（除了第一次 Activator.CreateInstance，实例创建完就一直用）
- **内存占用很小**：生成的程序集不大，一直用到程序结束，不需要卸载

所以性能完全不用担心，启动增加几十毫秒编译时间，运行时零开销。

## 为什么用动态生成而不用源生成器

.NET 现在有源生成器，为什么不用源生成器？

- 源生成器需要编译时处理，AntFlowCore 设计成类库，你的项目引用它，源生成器不好处理类库中的接口
- 动态生成更灵活，运行时根据实际程序集中的类型生成，完全按需生成
- Natasha 已经做得很好了，API 简单，几行代码搞定

当然，如果是完全 AOT 发布，可以改成源生成器，但是对于一般场景，动态生成更方便。

## 延伸：AutoParse 是怎么工作的

`[AutoParse]` 更进一步，它自动帮你生成基于枚举的 switch case：

比如你的方法：
```csharp
[AutoParse]
BpmnElementAdaptor GetBpmnElementAdaptor(NodePropertyEnum nodePropertyEnum);
```

它会自动：
1. 查找所有继承自 `BpmnElementAdaptor` 并且有对应的枚举标记的实现类
2. 生成 `switch (nodePropertyEnum)` 代码，每个 case 返回对应的实例
3. 你完全不需要写这个 switch

这就是把自动化做到极致了！

## 常见问题

### 1. 接口新增方法需要重新生成吗？

不需要，每次启动都是重新读取接口信息重新生成，所以新增方法直接加，重启就生效了。

### 2. 支持泛型方法吗？

支持！代码中已经处理了泛型，会正确替换泛型参数：

```csharp
if (returnType.IsGenericType)
{
    typeName = returnType.GetGenericTypeDefinition().FullName;
    var genericArguments = returnType.GetGenericArguments();
    if (genericArguments.Length > 0)
    {
        returnTypeName = typeName.Replace("`1", $"<{genericArguments[0].FullName}>");
    }
}
```

### 3. 编译失败怎么办？

如果接口方法定义不正确，或者特性标记错误，Natasha 编译会失败，会抛出异常，告诉你哪里错了，你根据错误信息修正接口定义就行。

### 4. 这个设计可以用到我的项目中吗？

完全可以！这是一个非常通用的设计，任何工厂模式场景，尤其是那些根据类型/枚举返回不同实现的场景，都可以用这个方式，省去大量手写代码。只需要引入 Natasha 包，照猫画虎写就行了。

## 总结

AntFlowCore 这个设计真的很巧妙：

- **问题**：工厂模式手写大量样板代码，违反开闭原则
- **解决方案**：使用 Natasha 动态编译，根据接口和特性自动生成工厂实现
- **优势**：新增实现只需要加特性，不需要改工厂代码，零样板代码，符合开闭原则
- **性能**：一次编译，终生使用，运行时性能和手写一样

这个设计充分发挥了 Natasha 的优势，把动态编译用在了刀刃上，让代码变得非常干净。这就是我认为最值得学习的一个高级技巧。

---

**相关链接：**
- [Natasha 官方 GitHub](https://github.com/dotnetcore/Natasha)
- [AntFlowCore 中 Natasha 的妙用：动态实现 IAdaptor 工厂](./AntFlowCore-Natasha-低代码条件计算.md)
- [AntFlowCore 事务处理详解](./AntFlowCore-事务处理详解-AOP-UnitOfWork.md)