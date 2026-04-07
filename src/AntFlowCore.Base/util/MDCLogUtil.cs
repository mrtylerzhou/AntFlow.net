using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Util
{
    public static class MDCLogUtil
    {
        private static readonly AsyncLocal<Dictionary<string, string>> _context =
            new AsyncLocal<Dictionary<string, string>>();

        /// <summary>
        /// 获取当前日志上下文中的 LogId。
        /// </summary>
        public static string GetLogId(ILogger logger)
        {
            try
            {
                if (_context.Value != null && _context.Value.TryGetValue("ruid", out string logId))
                {
                    return logId;
                }
                return SetLogId(logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while acquiring LogId.");
                return string.Empty;
            }
        }

        /// <summary>
        /// 重置日志上下文中的 LogId。
        /// </summary>
        public static void ResetLogId(ILogger logger)
        {
            CleanLogId();
            SetLogId(logger);
        }

        /// <summary>
        /// 设置新的 LogId，并存储到上下文中。
        /// </summary>
        private static string SetLogId(ILogger logger)
        {
            try
            {
                string ip = "_0";
                List<string> networkIPs = NetworkUtil.GetNetworkIPList();
                foreach (var ipAddress in networkIPs)
                {
                    if (ipAddress != "127.0.0.1")
                    {
                        ip = "_" + ipAddress.Substring(ipAddress.LastIndexOf('.') + 1);
                    }
                }

                string logId = ShortUUID.Generate() + ip;
                AddToContext("ruid", logId);
                logger.LogDebug("Generated new LogId: {LogId}", logId);
                return logId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while setting LogId.");
                return string.Empty;
            }
        }

        /// <summary>
        /// 清理日志上下文中的 LogId。
        /// </summary>
        public static void CleanLogId()
        {
            if (_context.Value != null)
            {
                _context.Value.Remove("ruid");
            }
        }

        /// <summary>
        /// 添加键值对到上下文。
        /// </summary>
        private static void AddToContext(string key, string value)
        {
            if (_context.Value == null)
            {
                _context.Value = new Dictionary<string, string>();
            }
            _context.Value[key] = value;
        }
    }

  
    /// <summary>
    /// 简短 UUID 生成器。
    /// </summary>
    public static class ShortUUID
    {
        public static string Generate()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8); // 生成短 UUID
        }
    }
}
