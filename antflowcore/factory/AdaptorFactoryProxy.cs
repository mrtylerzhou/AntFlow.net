using antflowcore.exception;

namespace antflowcore.factory;

using System;
using System.Linq;
using System.Reflection;
using System.Text;

public class AdaptorFactoryProxy
{
    static AdaptorFactoryProxy()
    {
        NatashaManagement
            //获取链式构造器
            .GetInitializer()
            //使用引用程序集中的命名空间
            .WithMemoryUsing()
            //使用内存中的元数据
            .WithMemoryReference()
            //注册域构造器
            .Preheating<NatashaDomainCreator>();
    }

    private static readonly string SIMPLECLSNAME = "IAdaptorFactory";
    private static readonly string PROXYFREFIX = "Proxy";
    private static readonly string PROXYSUFFIX = "Impl";
    private static volatile object loadedInstance = null;

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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        return (IAdaptorFactory)loadedInstance;
    }

    // 生成代理对象
    private static IAdaptorFactory GetProxyObj()
    {
        if (loadedInstance != null)
        {
            return (IAdaptorFactory)loadedInstance;
        }

        // 获取接口类型
        var interfaceType = Type.GetType($"antflowcore.factory.{SIMPLECLSNAME}");
        if (interfaceType == null)
        {
            throw new InvalidOperationException($"Cannot find interface {SIMPLECLSNAME}");
        }

        // 生成代理类代码
        var proxyClassCode = GenerateProxyClass(interfaceType);

        // 使用 Natasha 的 AssemblyCSharpBuilder 来编译代码并生成代理类
        var assembly = new AssemblyCSharpBuilder()
                        .Add(proxyClassCode)  // 将代理类代码添加到编译中
                        .GetAssembly();  // 编译代码

        // 生成动态类型
        var proxyType = assembly.GetType($"{PROXYFREFIX}{SIMPLECLSNAME}{PROXYSUFFIX}");

        // 实例化代理对象
        loadedInstance = Activator.CreateInstance(proxyType);
        return (IAdaptorFactory)loadedInstance;
    }

    // 生成代理类的代码
    public static string GenerateProxyClass(Type interfaceType)
    {
        var className = $"{PROXYFREFIX}{SIMPLECLSNAME}{PROXYSUFFIX}";
        var methods = interfaceType.GetMethods();
        var sb = new StringBuilder();

        sb.AppendLine($"public class {className} : {interfaceType.FullName} {{");

        foreach (var method in methods)
        {
            var methodName = method.Name;
            var returnType = method.ReturnType;
            var parameters = method.GetParameters();

            // 动态生成参数列表和方法体
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
                    throw new AFBizException($"method:{methodName} has neither TagParser nor a AutoParseAttribute");
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
                    // 获取泛型参数并将其替换为具体的类型名
                    var genericArguments = returnType.GetGenericArguments();
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
                        tagParserTypeName = tagParserGenericTypeDefinition.FullName.Replace("`1", $"<{parameters[0].ParameterType.FullName}>");
                    }
                }
            }

            // 生成方法签名
            sb.AppendLine($"public {returnTypeName} {methodName}({string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");

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