using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace AntFlow.Core.Factory;

public class AutoParseProxyFactory
{
    private static readonly string SIMPLECLSNAME = "TagParser";
    private static readonly string PROXYFREFIX = "Proxy";
    private static readonly string PROXYSUFFIX = "Impl";
    private static volatile ConcurrentDictionary<string, object> loadedInstances = new();

    public static object GetProxyInstance(ParameterInfo[] parameters, Type returnType)
    {
        string className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
        if (!loadedInstances.ContainsKey(className))
        {
            lock (className)
            {
                try
                {
                    return GetProxyObj(parameters, returnType);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        return loadedInstances[className];
    }

    // 获取代理对象
    private static object GetProxyObj(ParameterInfo[] parameters, Type returnType)
    {
        string className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
        if (loadedInstances.ContainsKey(className))
        {
            return loadedInstances[className];
        }

        // 创建泛型类型
        Type tagParserType = typeof(TagParser<,>);
        List<Type> paramsTypeArr = parameters.Select(a => a.ParameterType).ToList();
        paramsTypeArr.Insert(0, returnType);

        Type tagParserGenericType = tagParserType.MakeGenericType(paramsTypeArr.ToArray());

        // 生成代理类代码
        string? proxyClassCode = GenerateProxyClass(tagParserGenericType, parameters, returnType);

        // 使用 Natasha 的 AssemblyCSharpBuilder 编译生成程序集
        Assembly? assembly = new AssemblyCSharpBuilder()
            .Add(proxyClassCode) // 添加生成的代理类代码
            .GetAssembly(); // 获取程序集

        // 获取代理类型
        Type? proxyType = assembly.GetType($"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}");

        // 创建实例对象
        object? instance = Activator.CreateInstance(proxyType);
        loadedInstances.TryAdd(className, instance);
        return instance;
    }

    // 生成代理类代码
    public static string GenerateProxyClass(Type interfaceType, ParameterInfo[] parameters, Type returnType)
    {
        string? className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
        MethodInfo[]? methods = interfaceType.GetMethods();

        string varName = char.ToLower(returnType.Name[0]) + returnType.Name.Substring(1);
        string varNames = returnType.Name + "s";

        StringBuilder? sb = new();
        sb.AppendLine(
            $"public class {className} : AntFlow.Core.Factory.TagParser<{returnType.FullName},{string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName}"))}> {{");

        foreach (MethodInfo? method in methods)
        {
            string? methodName = method.Name;


            // 生成方法签名
            sb.AppendLine(
                $"public {returnType.FullName} {methodName}({string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");

            // 生成方法体
            sb.AppendLine("{");
            string body =
                "    {\n" +
                $"        if({string.Join(", ", parameters.Select(p => $" {p.Name}"))}==null){{\n" +
                "            throw new  AntFlow.Core.Exception.AFBizException(\"provided data to find a bpmnNodeAdaptor method is null\");\n" +
                "        }\n" +
                "\n" +
                $"        var {varNames} = AntFlow.Core.Util.ServiceProviderUtils.GetServices<AntFlow.Core.Adaptor.IAdaptorService>();\n" +
                $"        foreach (var {varName} in {varNames}) {{\n" +
                $"            if({varName}.IsSupportBusinessObject({string.Join(", ", parameters.Select(p => $" {p.Name}"))})){{\n" +
                $"                return ({returnType.FullName}){varName};\n" +
                "            }\n" +
                "        }\n" +
                "        return null;}\n";
            sb.AppendLine(body);
            sb.AppendLine("}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}