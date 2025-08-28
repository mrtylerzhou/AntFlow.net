using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Service;
using System.Collections;
using System.Collections.Concurrent;

namespace AntFlow.Core.Util;

public static class ServiceProviderUtils
{
    private static IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<string, object> CachedObjects = new();

    /// <summary>
    ///     初始化服务提供者，必须在应用启动时调用
    /// </summary>
    /// <param name="serviceProvider">ASP.NET Core 的 IServiceProvider 实例</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    ///     根据服务名称获取指定类型的服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="serviceName">服务名称</param>
    /// <returns>服务实例</returns>
    public static T GetService<T>(string serviceName)
    {
        // 获取所有指定类型的服务
        IEnumerable<T>? services = _serviceProvider.GetServices<T>();

        // 遍历所有服务实例
        foreach (T? service in services)
        {
            string? name = service.GetType().Name;

            // 比较服务名称
            if (name.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            {
                return service;
            }
        }

        throw new System.Exception($"No service with name '{serviceName}' found.");
    }


    public static object GetService(Type serviceType)
    {
        object? service = _serviceProvider.GetService(serviceType);
        return service ?? throw new System.Exception($"No service with type '{serviceType.Name}' found.");
    }

    /// <summary>
    ///     获取指定类型的必需服务实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    public static T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    ///     获取指定类型的所有服务实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例集合</returns>
    public static IEnumerable<T> GetServices<T>()
    {
        return _serviceProvider.GetServices<T>();
    }

    public static IEnumerable GetServices(Type type)
    {
        return _serviceProvider.GetServices(type);
    }

    /// <summary>
    ///     获取指定类型的所有有序服务实例
    /// </summary>
    /// <typeparam name="T">继承 IOrderedService 接口的服务类型</typeparam>
    /// <returns>按顺序排列的服务实例集合</returns>
    public static IEnumerable<T> GetOrderedServices<T>() where T : IOrderedService
    {
        return _serviceProvider.GetServices<T>().OrderBy(bean => bean.Order());
    }

    /// <summary>
    ///     缓存对象
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="obj">缓存对象</param>
    public static void PutObject(string key, object obj)
    {
        CachedObjects[key] = obj;
    }

    /// <summary>
    ///     获取缓存对象
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>缓存对象</returns>
    public static object GetObject(string key)
    {
        return CachedObjects.TryGetValue(key, out object? obj) ? obj : null;
    }

    public static IEnumerable<object> GetServicesByOpenGenericType(Type openGenericType)
    {
        IServiceCollection? serviceCollection = ServiceCollectionHolder.Services;

        // 查找匹配的泛型服务描述符
        IEnumerable<Type>? matchingDescriptors = serviceCollection
            .Where(descriptor => descriptor.ServiceType.IsGenericType &&
                                 descriptor.ServiceType.GetGenericTypeDefinition() == openGenericType)
            .Select(descriptor => descriptor.ServiceType);

        // 从 IServiceProvider 获取服务实例
        foreach (Type? serviceType in matchingDescriptors)
        {
            object? serviceInstance = _serviceProvider.GetService(serviceType);
            if (serviceInstance != null)
            {
                yield return serviceInstance;
            }
        }
    }
}