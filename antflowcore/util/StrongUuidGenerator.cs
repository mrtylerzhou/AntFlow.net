using System.Diagnostics;

namespace antflowcore.util;

using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

    using System;
using System.Linq;
using System.Net.NetworkInformation;

 public static class StrongUuidGenerator
    {
        private static readonly TimeBasedGuidGenerator Generator;

        static StrongUuidGenerator()
        {
            var ethernet = GetEthernetAddress();
            Generator = new TimeBasedGuidGenerator(ethernet);
        }

        public static string GetNextId()
        {
            return Generator.GenerateGuid().ToString();
        }

        private static byte[] GetEthernetAddress()
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    var address = nic.GetPhysicalAddress();
                    if (address != null && address.GetAddressBytes().Length == 6)
                    {
                        return address.GetAddressBytes();
                    }
                }
            }

            // fallback: random MAC
            var rnd = new Random();
            byte[] mac = new byte[6];
            rnd.NextBytes(mac);
            mac[0] |= 0x01;
            return mac;
        }
    }

    internal class TimeBasedGuidGenerator
    {
        private static long _lastTimestamp = 0;
        private static int _sequence = 0;
        private readonly byte[] _nodeId;

        public TimeBasedGuidGenerator(byte[] nodeId)
        {
            _nodeId = nodeId ?? throw new ArgumentNullException(nameof(nodeId));
        }

        public Guid GenerateGuid()
        {
            long timestamp = GetUniqueTimestamp();
            byte[] guidBytes = new byte[16];

            // 时间戳填前8位
            byte[] timeBytes = BitConverter.GetBytes(timestamp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeBytes);
            Array.Copy(timeBytes, 0, guidBytes, 0, 8);

            // MAC 地址填后6位
            Array.Copy(_nodeId, 0, guidBytes, 10, 6);

            // 序列号填中间2位
            int seq = Interlocked.Increment(ref _sequence) & 0xFFFF;
            guidBytes[8] = (byte)(seq >> 8);
            guidBytes[9] = (byte)(seq & 0xFF);

            return new Guid(guidBytes);
        }

        private static long GetUniqueTimestamp()
        {
            while (true)
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long last = Interlocked.Read(ref _lastTimestamp);

                if (now == last)
                {
                    // 同一毫秒，强制增加
                    now = Interlocked.Increment(ref _lastTimestamp);
                    return now;
                }

                if (Interlocked.CompareExchange(ref _lastTimestamp, now, last) == last)
                {
                    return now;
                }
            }
        }
    }
