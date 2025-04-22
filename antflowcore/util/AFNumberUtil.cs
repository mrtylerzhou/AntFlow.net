namespace antflowcore.util;

using Murmur;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

public static class AFNumberUtil
{
    // 62 进制字符集：0-9, A-Z, a-z
    private static readonly char[] Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    private const int Base = 62;

    /// <summary>
    /// 将长整数转换为 62 进制字符串
    /// </summary>
    /// <param name="number">原始长整数</param>
    /// <returns>转换后的 62 进制字符串</returns>
    public static string EncodeLong(long number)
    {
        if (number < 0)
            throw new ArgumentException("Number must be non-negative.");

        var encoded = new StringBuilder();
        do
        {
            int remainder = (int)(number % Base);
            encoded.Append(Base62Chars[remainder]);
            number /= Base;
        } while (number > 0);

        // 字符串需要反转，因为我们是从低位到高位生成的
        var charArray = encoded.ToString().ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// 将 62 进制字符串解码回长整数
    /// </summary>
    /// <param name="encoded">62 进制字符串</param>
    /// <returns>解码后的长整数</returns>
    public static long DecodeLong(string encoded)
    {
        if (string.IsNullOrEmpty(encoded))
            throw new ArgumentException("Encoded string cannot be null or empty.");

        long number = 0;
        foreach (var c in encoded)
        {
            int value = CharToValue(c);
            number = number * Base + value;
        }

        return number;
    }

    /// <summary>
    /// 将业务 ID 转换为短格式
    /// </summary>
    /// <param name="businessId">原始业务 ID</param>
    /// <returns>短格式业务 ID</returns>
    public static string ShortBusinessId(string businessId)
    {
        if (businessId == null) return null;
        return businessId.Length > 10 ? GenerateShortHash(businessId) : businessId;
    }

    /// <summary>
    /// 使用 MurmurHash 生成哈希值并转换为 Base62
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>短哈希值</returns>
    public static string GenerateShortHash(string input)
    {
        var murmur128 = MurmurHash.Create128(managed: true);
        byte[] hashBytes = murmur128.ComputeHash(Encoding.UTF8.GetBytes(input));
        long murmurHash = BitConverter.ToInt64(hashBytes, 0);
        murmurHash = murmurHash < 0 ? -murmurHash : murmurHash; // 确保为正数
        return EncodeLong(murmurHash);
    }

    /// <summary>
    /// 将字符串的 MD5 转换为 Base62
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>Base62 编码字符串</returns>
    public static string Md5ToBase62(string input)
    {
        using var md5 = MD5.Create();
        byte[] md5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

        // 转为大整数
        var bigInt = new BigInteger(md5Bytes.Concat(new byte[] { 0 }).ToArray()); // 确保为非负数

        // 转为 62 进制字符串
        var base62 = new StringBuilder();
        while (bigInt > 0)
        {
            int remainder = (int)(bigInt % Base);
            base62.Append(Base62Chars[remainder]);
            bigInt /= Base;
        }

        // 字符串逆序（因为取余从低位开始生成）
        var charArray = base62.ToString().ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// 将字符转换为对应的值
    /// </summary>
    /// <param name="c">输入字符</param>
    /// <returns>对应的数值</returns>
    private static int CharToValue(char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        if (c >= 'A' && c <= 'Z')
            return c - 'A' + 10;
        if (c >= 'a' && c <= 'z')
            return c - 'a' + 36;

        throw new ArgumentException($"Invalid character in encoded string: {c}");
    }
}