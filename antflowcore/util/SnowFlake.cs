namespace antflowcore.util;

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

public class SnowFlake
{
    private static readonly long Twepoch = 12888349746579L;
    private static readonly long WorkerIdBits = 5L; // 机器标识位数
    private static readonly long DatacenterIdBits = 5L; // 数据中心标识位数
    private static readonly long SequenceBits = 12L; // 毫秒内序列位数

    private static readonly long MaxWorkerId = (-1L ^ (-1L << (int)WorkerIdBits)); // 最大机器ID
    private static readonly long MaxDatacenterId = (-1L ^ (-1L << (int)DatacenterIdBits)); // 最大数据中心ID

    private static readonly long WorkerIdShift = SequenceBits; // 机器ID左移位数
    private static readonly long DatacenterIdShift = SequenceBits + WorkerIdBits; // 数据中心ID左移位数
    private static readonly long TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits; // 时间戳左移位数
    private static readonly long SequenceMask = -1L ^ (-1L << (int)SequenceBits); // 序列号掩码

    private static long WorkerId; // 机器ID
    private static long DatacenterId; // 数据中心ID
    private static long Sequence = 0L; // 当前毫秒内序列
    private static long LastTimestamp = -1L; // 上次生成ID的时间戳

    // 静态初始化 WorkerId 和 DatacenterId
    static SnowFlake()
    {
        WorkerId = GetWorkerId();
        DatacenterId = GetDatacenterId();
    }

    /// <summary>
    /// 生成下一个唯一 ID（静态方法）。
    /// </summary>
    public static long NextId()
    {
        lock (typeof(SnowFlake)) // 锁定类，防止并发问题
        {
            long timestamp = TimeGen();

            if (timestamp < LastTimestamp)
            {
                throw new InvalidOperationException(
                    string.Format("Clock moved backwards. Refusing to generate id for {0} milliseconds", LastTimestamp - timestamp));
            }

            if (LastTimestamp == timestamp)
            {
                // 同一毫秒内，序列号自增
                Sequence = (Sequence + 1) & SequenceMask;
                if (Sequence == 0)
                {
                    // 序列号用尽，等待下一毫秒
                    timestamp = TilNextMillis(LastTimestamp);
                }
            }
            else
            {
                Sequence = 0L;
            }

            LastTimestamp = timestamp;

            return ((timestamp - Twepoch) << (int)TimestampLeftShift) |
                   (DatacenterId << (int)DatacenterIdShift) |
                   (WorkerId << (int)WorkerIdShift) |
                   Sequence;
        }
    }

    /// <summary>
    /// 阻塞等待直到下一毫秒。
    /// </summary>
    private static long TilNextMillis(long lastTimestamp)
    {
        long timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }
        return timestamp;
    }

    /// <summary>
    /// 获取当前时间戳（毫秒）。
    /// </summary>
    private static long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取机器 ID。
    /// </summary>
    private static long GetWorkerId()
    {
        try
        {
            string podName = Environment.GetEnvironmentVariable("POD_NAME");
            if (podName != null)
            {
                return Math.Abs(podName.GetHashCode()) & MaxWorkerId;
            }

            var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            return Math.Abs(ip.GetHashCode()) & MaxWorkerId;
        }
        catch (Exception)
        {
            return 0L;
        }
    }

    /// <summary>
    /// 获取数据中心 ID。
    /// </summary>
    private static long GetDatacenterId()
    {
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var sb = new StringBuilder();
            foreach (var ni in networkInterfaces)
            {
                var mac = ni.GetPhysicalAddress().GetAddressBytes();
                if (mac != null)
                {
                    foreach (var b in mac)
                    {
                        sb.AppendFormat("{0:X2}", b);
                    }
                }
            }
            return Math.Abs(sb.ToString().GetHashCode()) & MaxDatacenterId;
        }
        catch (Exception)
        {
            return 0L;
        }
    }
}