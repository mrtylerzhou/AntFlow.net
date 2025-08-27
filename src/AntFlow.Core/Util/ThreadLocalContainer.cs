using System.Collections.Concurrent;

namespace AntFlow.Core.Util;

public static class ThreadLocalContainer
{
    // 使用 ThreadLocal 存储线程本地数据
    private static readonly ThreadLocal<ConcurrentDictionary<string, object>> _cache =
        new(() => new ConcurrentDictionary<string, object>());

    /// <summary>
    ///     获取线程本地存储的值
    /// </summary>
    /// <param name="key">键名</param>
    /// <returns>对应的值，如果不存在则返回 null</returns>
    public static object Get(string key)
    {
        return _cache.Value.TryGetValue(key, out object? value) ? value : null;
    }

    /// <summary>
    ///     设置线程本地存储的值
    /// </summary>
    /// <param name="key">键名</param>
    /// <param name="val">值</param>
    public static void Set(string key, object val)
    {
        _cache.Value[key] = val;
    }

    /// <summary>
    ///     移除线程本地存储的值
    /// </summary>
    /// <param name="key">键名</param>
    /// <returns>被移除的值，如果不存在则返回 null</returns>
    public static object Remove(string key)
    {
        _cache.Value.TryRemove(key, out object? value);
        return value;
    }

    /// <summary>
    ///     清空当前线程的所有本地存储数据
    /// </summary>
    /// <returns>清空前的所有数据副本</returns>
    public static IDictionary<string, object> Clean()
    {
        Dictionary<string, object>? map = new(_cache.Value);
        _cache.Value.Clear(); // 清空缓存
        return map;
    }
}