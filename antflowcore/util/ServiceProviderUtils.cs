using System.Collections;
using System.Collections.Concurrent;
using antflowcore.conf.di;
using antflowcore.service;
using Microsoft.Extensions.DependencyInjection;

namespace antflowcore.util;

public static class ServiceProviderUtils
{
    private static IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<string, object> CachedObjects = new();

    /// <summary>
    /// 初始化服务提供者（通常在应用启动时调用）。
    /// </summary>
    /// <param name="serviceProvider">ASP.NET Core 的 IServiceProvider 实例。</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// 获取一个服务实例（通过名称）。
    /// </summary>
    /// <typeparam name="T">服务类型。</typeparam>
    /// <param name="serviceName">服务名称。</param>
    /// <returns>服务实例。</returns>
    public static T GetService<T>(string serviceName)
    {
        // 获取所有服务实例
        var services = _serviceProvider.GetServices<T>();

        // 根据服务名称筛选
        foreach (var service in services)
        {
            var name = service.GetType().Name;

            // 比较名称
            if (name.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            {
                return service;
            }
        }

        throw new Exception($"No service with name '{serviceName}' found.");
    }


    public static Object GetService(Type serviceType)
    {
        var service = _serviceProvider.GetService(serviceType);
        return service ?? throw new Exception($"No service with type '{serviceType.Name}' found.");
    }
    /// <summary>
    /// 获取一个服务实例（通过类型）。
    /// </summary>
    /// <typeparam name="T">服务类型。</typeparam>
    /// <returns>服务实例。</returns>
    public static T GetService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 获取所有指定类型的服务实例。
    /// </summary>
    /// <typeparam name="T">服务类型。</typeparam>
    /// <returns>所有服务实例的集合。</returns>
    public static IEnumerable<T> GetServices<T>()
    {
        return _serviceProvider.GetServices<T>();
    }

    public static IEnumerable GetServices(Type type)
    {
        return _serviceProvider.GetServices(type);
    }

    /// <summary>
    /// 获取所有指定类型的服务实例，并按顺序排列。
    /// </summary>
    /// <typeparam name="T">实现 IOrderedBean 接口的服务类型。</typeparam>
    /// <returns>按顺序排列的服务实例集合。</returns>
    public static IEnumerable<T> GetOrderedServices<T>() where T : IOrderedService
    {
        return _serviceProvider.GetServices<T>().OrderBy(bean => bean.Order());
    }

    /// <summary>
    /// 添加缓存的对象。
    /// </summary>
    /// <param name="key">对象的唯一键。</param>
    /// <param name="obj">缓存的对象。</param>
    public static void PutObject(string key, object obj)
    {
        CachedObjects[key] = obj;
    }

    /// <summary>
    /// 获取缓存的对象。
    /// </summary>
    /// <param name="key">对象的唯一键。</param>
    /// <returns>缓存的对象。</returns>
    public static object GetObject(string key)
    {
        return CachedObjects.TryGetValue(key, out var obj) ? obj : null;
    }

    public static IEnumerable<object> GetServicesByOpenGenericType(Type openGenericType)
    {
        
        var serviceCollection = ServiceCollectionHolder.Services;

        // 筛选出所有符合开放泛型定义的服务
        var matchingDescriptors = serviceCollection
            .Where(descriptor => descriptor.ServiceType.IsGenericType &&
                                 descriptor.ServiceType.GetGenericTypeDefinition() == openGenericType)
            .Select(descriptor => descriptor.ServiceType);

        // 通过 IServiceProvider 获取服务实例
        foreach (var serviceType in matchingDescriptors)
        {
            var serviceInstance = _serviceProvider.GetService(serviceType);
            if (serviceInstance != null)
            {
                yield return serviceInstance;
            }
        }
    }
}