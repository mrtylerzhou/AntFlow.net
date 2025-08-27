using Murmur;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace AntFlow.Core.Util;

public static class AFNumberUtil
{
    private const int Base = 62;

    // 62 �����ַ�����0-9, A-Z, a-z
    private static readonly char[] Base62Chars =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    /// <summary>
    ///     ��������ת��Ϊ 62 �����ַ���
    /// </summary>
    /// <param name="number">ԭʼ������</param>
    /// <returns>ת����� 62 �����ַ���</returns>
    public static string EncodeLong(long number)
    {
        if (number < 0)
        {
            throw new ArgumentException("Number must be non-negative.");
        }

        StringBuilder? encoded = new();
        do
        {
            int remainder = (int)(number % Base);
            encoded.Append(Base62Chars[remainder]);
            number /= Base;
        } while (number > 0);

        // �ַ�����Ҫ��ת����Ϊ�����Ǵӵ�λ����λ���ɵ�
        char[]? charArray = encoded.ToString().ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    ///     �� 62 �����ַ�������س�����
    /// </summary>
    /// <param name="encoded">62 �����ַ���</param>
    /// <returns>�����ĳ�����</returns>
    public static long DecodeLong(string encoded)
    {
        if (string.IsNullOrEmpty(encoded))
        {
            throw new ArgumentException("Encoded string cannot be null or empty.");
        }

        long number = 0;
        foreach (char c in encoded)
        {
            int value = CharToValue(c);
            number = (number * Base) + value;
        }

        return number;
    }

    /// <summary>
    ///     ��ҵ�� ID ת��Ϊ�̸�ʽ
    /// </summary>
    /// <param name="businessId">ԭʼҵ�� ID</param>
    /// <returns>�̸�ʽҵ�� ID</returns>
    public static string ShortBusinessId(string businessId)
    {
        if (businessId == null)
        {
            return null;
        }

        return businessId.Length > 10 ? GenerateShortHash(businessId) : businessId;
    }

    /// <summary>
    ///     ʹ�� MurmurHash ���ɹ�ϣֵ��ת��Ϊ Base62
    /// </summary>
    /// <param name="input">�����ַ���</param>
    /// <returns>�̹�ϣֵ</returns>
    public static string GenerateShortHash(string input)
    {
        Murmur128? murmur128 = MurmurHash.Create128(managed: true);
        byte[] hashBytes = murmur128.ComputeHash(Encoding.UTF8.GetBytes(input));
        long murmurHash = BitConverter.ToInt64(hashBytes, 0);
        murmurHash = murmurHash < 0 ? -murmurHash : murmurHash; // ȷ��Ϊ����
        return EncodeLong(murmurHash);
    }

    /// <summary>
    ///     ���ַ����� MD5 ת��Ϊ Base62
    /// </summary>
    /// <param name="input">�����ַ���</param>
    /// <returns>Base62 �����ַ���</returns>
    public static string Md5ToBase62(string input)
    {
        using MD5? md5 = MD5.Create();
        byte[] md5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

        // תΪ������
        BigInteger bigInt = new(md5Bytes.Concat(new byte[] { 0 }).ToArray()); // ȷ��Ϊ�Ǹ���

        // תΪ 62 �����ַ���
        StringBuilder? base62 = new();
        while (bigInt > 0)
        {
            int remainder = (int)(bigInt % Base);
            base62.Append(Base62Chars[remainder]);
            bigInt /= Base;
        }

        // �ַ���������Ϊȡ��ӵ�λ��ʼ���ɣ�
        char[]? charArray = base62.ToString().ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    ///     ���ַ�ת��Ϊ��Ӧ��ֵ
    /// </summary>
    /// <param name="c">�����ַ�</param>
    /// <returns>��Ӧ����ֵ</returns>
    private static int CharToValue(char c)
    {
        if (c >= '0' && c <= '9')
        {
            return c - '0';
        }

        if (c >= 'A' && c <= 'Z')
        {
            return c - 'A' + 10;
        }

        if (c >= 'a' && c <= 'z')
        {
            return c - 'a' + 36;
        }

        throw new ArgumentException($"Invalid character in encoded string: {c}");
    }
}