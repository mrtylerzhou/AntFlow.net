using System.Collections.Concurrent;
using antflowcore.adaptor;
using antflowcore.util;

namespace antflowcore.factory;

using Natasha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

    public class AutoParseProxyFactory
    {
       
        private static readonly string SIMPLECLSNAME = "TagParser";
        private static readonly string PROXYFREFIX = "Proxy";
        private static readonly string PROXYSUFFIX = "Impl";
        private static volatile ConcurrentDictionary<string,object> loadedInstances = new ConcurrentDictionary<string, object>();
        public static object GetProxyInstance(ParameterInfo[] parameters,Type returnType)
        {
            string className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
            if (!loadedInstances.ContainsKey(className))
            {
                lock (className)
                {
                    try
                    {
                        return GetProxyObj(parameters,returnType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            return loadedInstances[className];
        }

        // 生成代理对象
        private static object GetProxyObj(ParameterInfo[] parameters,Type returnType)
        {
            string className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
            if (loadedInstances.ContainsKey(className))
            {
                return loadedInstances[className];
            }

            // 获取接口类型
            Type tagParserType = typeof(TagParser<,>);
            List<Type> paramsTypeArr = parameters.Select(a=>a.ParameterType).ToList();
            paramsTypeArr.Insert(0,returnType);

            Type tagParserGenericType = tagParserType.MakeGenericType(paramsTypeArr.ToArray());

            // 生成代理类代码
            var proxyClassCode = GenerateProxyClass(tagParserGenericType,parameters,returnType);

            // 使用 Natasha 的 AssemblyCSharpBuilder 来编译代码并生成代理类
            var assembly = new AssemblyCSharpBuilder()
                            .Add(proxyClassCode)  // 将代理类代码添加到编译中
                            .GetAssembly();  // 编译代码

            // 生成动态类型
            var proxyType = assembly.GetType($"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}");

            // 实例化代理对象
            var instance = Activator.CreateInstance(proxyType);
            loadedInstances.TryAdd(className,instance);
            return instance;
        }

        // 生成代理类的代码
        public static string GenerateProxyClass(Type interfaceType,ParameterInfo[] parameters,Type returnType)
        {
            var className = $"{PROXYFREFIX}{returnType.Name}{SIMPLECLSNAME}{PROXYSUFFIX}";
            var methods = interfaceType.GetMethods();
            
            string varName=char.ToLower(returnType.Name[0]) + returnType.Name.Substring(1);
            string varNames=returnType.Name+"s";
            
            var sb = new StringBuilder();
            sb.AppendLine($"public class {className} : antflowcore.factory.TagParser<{returnType.FullName},{string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName}"))}> {{");

            foreach (var method in methods)
            {
                
                var methodName = method.Name;
                

                // 生成方法签名
                sb.AppendLine($"public {returnType.FullName} {methodName}({string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");
               
                // 生成方法体
                sb.AppendLine("{");
                String body =
                    "    {\n" +
                    $"        if({string.Join(", ", parameters.Select(p => $" {p.Name}"))}==null){{\n" +
                    "            throw new  antflowcore.exception.AFBizException(\"provided data to find a bpmnNodeAdaptor method is null\");\n" +
                    "        }\n" +
                    "\n" +
                    $"        var {varNames} = antflowcore.util.ServiceProviderUtils.GetServices<antflowcore.adaptor.IAdaptorService>();\n" +
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
    
    