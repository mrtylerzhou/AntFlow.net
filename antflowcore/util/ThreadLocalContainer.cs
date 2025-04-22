using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AntFlowCore.Util
{
    public static class ThreadLocalContainer
    {
        // 使用 ThreadLocal 来存储线程局部数据
        private static ThreadLocal<ConcurrentDictionary<string, object>> _cache = 
            new ThreadLocal<ConcurrentDictionary<string, object>>(() => new ConcurrentDictionary<string, object>());

        /// <summary>
        /// 获取线程本地缓存中的值
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <returns>缓存的值，如果没有找到则为 null</returns>
        public static object Get(string key)
        {
            return _cache.Value.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// 设置线程本地缓存的值
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <param name="val">缓存的值</param>
        public static void Set(string key, object val)
        {
            _cache.Value[key] = val;
        }

        /// <summary>
        /// 移除线程本地缓存中的值
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <returns>移除的值，如果没有找到则为 null</returns>
        public static object Remove(string key)
        {
            _cache.Value.TryRemove(key, out var value);
            return value;
        }

        /// <summary>
        /// 清空当前线程的缓存，并返回已存储的数据
        /// </summary>
        /// <returns>返回当前线程所有的缓存数据</returns>
        public static IDictionary<string, object> Clean()
        {
            var map = new Dictionary<string, object>(_cache.Value);
            _cache.Value.Clear(); // 清空线程本地存储
            return map;
        }
    }
}