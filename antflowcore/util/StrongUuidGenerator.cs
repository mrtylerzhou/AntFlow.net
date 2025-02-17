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
    private static readonly TimeBasedGenerator TimeBasedGenerator;

    static StrongUuidGenerator()
    {
        var ethernetAddress = GetEthernetAddress();
        TimeBasedGenerator = new TimeBasedGenerator(ethernetAddress, new UuidTimer());
    }

    private static EthernetAddress GetEthernetAddress()
    {
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                nic.OperationalStatus == OperationalStatus.Up)
            {
                var address = nic.GetPhysicalAddress();
                if (address != null && address.GetAddressBytes().Length == 6)
                {
                    return new EthernetAddress(address.GetAddressBytes());
                }
            }
        }

        var random = new Random();
        byte[] randomAddress = new byte[6];
        random.NextBytes(randomAddress);
        randomAddress[0] |= 0x01; // 确保是多播地址
        return new EthernetAddress(randomAddress);
    }

    public static string GetNextId()
    {
        return TimeBasedGenerator.Generate().ToString();
    }
}


public class TimeBasedGenerator
{
    private readonly EthernetAddress _ethernetAddress;
    private readonly UuidTimer _timer;
    private readonly long _uuidL2;

    public TimeBasedGenerator(EthernetAddress ethernetAddress, UuidTimer timer)
    {
        byte[] uuidBytes = new byte[16];
        _ethernetAddress = ethernetAddress ?? EthernetAddress.ConstructMulticastAddress();
        _ethernetAddress.ToByteArray(uuidBytes, 10);

        int clockSeq = timer.GetClockSequence();
        uuidBytes[8] = (byte)(clockSeq >> 8);
        uuidBytes[9] = (byte)clockSeq;

        _uuidL2 = InitUuidSecondLong(uuidBytes);
        _timer = timer;
    }

    public Guid Generate()
    {
        long rawTimestamp = _timer.GetTimestamp();
        int clockHi = (int)(rawTimestamp >> 32);
        int clockLo = (int)rawTimestamp;

        int midhi = (clockHi << 16) | (clockHi >> 16);
        midhi &= ~0x3000; // 清除版本位
        midhi |= 0x1000;  // 设置为时间序列 UUID

        long l1 = ((long)clockLo << 32) | (uint)midhi;
        return new Guid(BitConverter.GetBytes(l1).Reverse().ToArray()
            .Concat(BitConverter.GetBytes(_uuidL2).Reverse().ToArray()).ToArray());
    }

    private static long InitUuidSecondLong(byte[] uuidBytes)
    {
        long l2 = ((long)uuidBytes[8] << 56)
                | ((long)uuidBytes[9] << 48)
                | ((long)uuidBytes[10] << 40)
                | ((long)uuidBytes[11] << 32)
                | ((long)uuidBytes[12] << 24)
                | ((long)uuidBytes[13] << 16)
                | ((long)uuidBytes[14] << 8)
                | uuidBytes[15];

        return (l2 << 2) >> 2 | long.MinValue;
    }
}

public class EthernetAddress
{
    private readonly byte[] _address;

    public EthernetAddress(byte[] address)
    {
        if (address == null || address.Length != 6)
        {
            throw new ArgumentException("Ethernet address must be exactly 6 bytes.");
        }
        _address = address;
    }

    public static EthernetAddress FromInterface()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in interfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                var address = networkInterface.GetPhysicalAddress()?.GetAddressBytes();
                if (address != null && address.Length == 6)
                {
                    return new EthernetAddress(address);
                }
            }
        }
        catch (Exception)
        {
        }

        return null;
    }

    public static EthernetAddress ConstructMulticastAddress()
    {
        return new EthernetAddress(new byte[] { 0x01, 0x00, 0x5E, 0x00, 0x00, 0x00 });
    }

    public void ToByteArray(byte[] buffer, int offset)
    {
        Array.Copy(_address, 0, buffer, offset, _address.Length);
    }
}

public class UuidTimer
{
    private static long _lastTimestamp;
    private static long _sequence = 0;
    private static int _clockSequence = 0;

    public UuidTimer()
    {
        _lastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public long GetTimestamp()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long lastTimestamp = Interlocked.Exchange(ref _lastTimestamp, timestamp);
        retrylbl:
        if (timestamp == lastTimestamp)
        {
            Interlocked.Increment(ref _sequence);
        }
        else
        {
            Interlocked.Exchange(ref _sequence, 0);
        }

        try
        {
            return checked(timestamp + _sequence);
        }
        catch (Exception e)
        {
            Interlocked.Exchange(ref _sequence, 0);
            goto retrylbl;
        }
    }

    public int GetClockSequence()
    {
        return checked(Interlocked.Increment(ref _clockSequence) & 0x3FFF);
    }
}
