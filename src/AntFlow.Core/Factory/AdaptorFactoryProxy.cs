using System.Reflection;
using System.Text;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Factory;

public class AdaptorFactoryProxy
{
    private static readonly string SIMPLECLSNAME = "IAdaptorFactory";
    private static readonly string PROXYFREFIX = "Proxy";
    private static readonly string PROXYSUFFIX = "Impl";
    private static volatile object loadedInstance;

    static AdaptorFactoryProxy()
    {
        NatashaManagement
            //获取初始化器
            .GetInitializer()
            //使用内存编译和内存引用
            .WithMemoryUsing()
            //使用内存引用模式
            .WithMemoryReference()
            //预热编译器
            .Preheating<NatashaDomainCreator>();
    }

    public static IAdaptorFactory GetProxyInstance()
    {
        if (loadedInstance == null)
        {
            lock (SIMPLECLSNAME)
            {
                try
                {
                    return GetProxyObj();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        return (IAdaptorFactory)loadedInstance;
    }

    // 获取代理对象
    private static IAdaptorFactory GetProxyObj()
    {
        if (loadedInstance != null)
        {
            return (IAdaptorFactory)loadedInstance;
        }

        // 获取接口类型
        Type? interfaceType = Type.GetType($"AntFlow.Core.Factory.{SIMPLECLSNAME}");
        if (interfaceType == null)
        {
            throw new InvalidOperationException($"Cannot find interface {SIMPLECLSNAME}");
        }

        // 生成代理类代码
        string? proxyClassCode = GenerateProxyClass(interfaceType);

        // 使用 Natasha 的 AssemblyCSharpBuilder 编译代理类
        Assembly? assembly = new AssemblyCSharpBuilder()
            .Add(proxyClassCode) // 添加代理类源代码
            .GetAssembly(); // 获取程序集

        // 获取代理类型
        Type? proxyType = assembly.GetType($"{PROXYFREFIX}{SIMPLECLSNAME}{PROXYSUFFIX}");

        // 创建代理实例
        loadedInstance = Activator.CreateInstance(proxyType);
        return (IAdaptorFactory)loadedInstance;
    }

    // 生成代理类代码
    public static string GenerateProxyClass(Type interfaceType)
    {
        string? className = $"{PROXYFREFIX}{SIMPLECLSNAME}{PROXYSUFFIX}";
        MethodInfo[]? methods = interfaceType.GetMethods();
        StringBuilder? sb = new();

        sb.AppendLine($"public class {className} : {interfaceType.FullName} {{");

        foreach (MethodInfo? method in methods)
        {
            string? methodName = method.Name;
            Type? returnType = method.ReturnType;
            ParameterInfo[]? parameters = method.GetParameters();

            // 构建方法参数声明字符串
            string parameterDeclaration = string.Join(", ", parameters.Select(p => $"{p.Name}"));


            SpfServiceAttribute? spfServiceAttribute = method.GetCustomAttribute<SpfServiceAttribute>();
            Type tagParserType = null;
            if (spfServiceAttribute != null)
            {
                tagParserType = spfServiceAttribute.TagParser;
            }
            else
            {
                AutoParseAttribute? autoParseAttribute = method.GetCustomAttribute<AutoParseAttribute>();
                if (autoParseAttribute == null)
                {
                    throw new AntFlowException.AFBizException(
                        $"method:{methodName} has neither TagParser nor a AutoParseAttribute");
                }

                object proxyInstance = AutoParseProxyFactory.GetProxyInstance(parameters, returnType);
                tagParserType = proxyInstance.GetType();
            }

            string tagParserTypeName = tagParserType.FullName;
            string returnTypeName = returnType.FullName;
            if (spfServiceAttribute != null)
            {
                string typeName = returnType.FullName;
                if (returnType.IsGenericType)
                {
                    typeName = returnType.GetGenericTypeDefinition().FullName;
                    // 处理泛型类型的完整类型名称
                    Type[]? genericArguments = returnType.GetGenericArguments();
                    if (genericArguments.Length > 0)
                    {
                        returnTypeName = typeName.Replace("`1", $"<{genericArguments[0].FullName}>");
                    }
                }

                if (tagParserType.IsGenericType)
                {
                    //todo opt
                    Type tagParserGenericTypeDefinition = tagParserType.GetGenericTypeDefinition();
                    if (tagParserGenericTypeDefinition != null)
                    {
                        tagParserTypeName =
                            tagParserGenericTypeDefinition.FullName.Replace("`1",
                                $"<{parameters[0].ParameterType.FullName}>");
                    }
                }
            }

            // 生成方法签名
            sb.AppendLine(
                $"public {returnTypeName} {methodName}({string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");

            // 生成方法体
            sb.AppendLine("{");
            sb.AppendLine($" dynamic instance =  System.Activator.CreateInstance(typeof({tagParserTypeName}));");
            sb.AppendLine($" return instance.ParseTag({parameterDeclaration});");
            sb.AppendLine("}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}