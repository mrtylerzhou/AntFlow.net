using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace AntFlowCore.Core.util;

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
        /// 使用 MurmurHash3 128-bit 生成哈希值并转换为 Base62
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>短哈希值</returns>
        public static string GenerateShortHash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MurmurHash3(data, 0);
            long hash = BitConverter.ToInt64(hashBytes, 0);
            hash = hash < 0 ? -hash : hash;
            return EncodeLong(hash);
        }

        private static byte[] MurmurHash3(byte[] data, uint seed)
        {
            const ulong c1 = 0x87c37b91114253d5;
            const ulong c2 = 0x4cf5ad432745937f;
            int length = data.Length;
            int nblocks = length / 16;
            ulong h1 = seed;
            ulong h2 = seed;

            for (int i = 0; i < nblocks; i++)
            {
                ulong bk1 = BitConverter.ToUInt64(data, i * 16);
                ulong bk2 = BitConverter.ToUInt64(data, i * 16 + 8);
                bk1 *= c1; bk1 = RotateLeft(bk1, 31); bk1 *= c2; h1 ^= bk1;
                h1 = RotateLeft(h1, 27); h1 += h2; h1 = h1 * 5 + 0x52dce729;
                bk2 *= c2; bk2 = RotateLeft(bk2, 33); bk2 *= c1; h2 ^= bk2;
                h2 = RotateLeft(h2, 31); h2 += h1; h2 = h2 * 5 + 0x38495ab5;
            }

            ulong k1 = 0, k2 = 0;
            int tail = nblocks * 16;
            switch (length & 15)
            {
                case 15: k2 ^= (ulong)data[tail + 14] << 48; goto case 14;
                case 14: k2 ^= (ulong)data[tail + 13] << 40; goto case 13;
                case 13: k2 ^= (ulong)data[tail + 12] << 32; goto case 12;
                case 12: k2 ^= (ulong)data[tail + 11] << 24; goto case 11;
                case 11: k2 ^= (ulong)data[tail + 10] << 16; goto case 10;
                case 10: k2 ^= (ulong)data[tail + 9] << 8; goto case 9;
                case 9: k2 ^= (ulong)data[tail + 8]; k2 *= c2; k2 = RotateLeft(k2, 33); k2 *= c1; h2 ^= k2;
                    goto case 8;
                case 8: k1 ^= (ulong)data[tail + 7] << 56; goto case 7;
                case 7: k1 ^= (ulong)data[tail + 6] << 48; goto case 6;
                case 6: k1 ^= (ulong)data[tail + 5] << 40; goto case 5;
                case 5: k1 ^= (ulong)data[tail + 4] << 32; goto case 4;
                case 4: k1 ^= (ulong)data[tail + 3] << 24; goto case 3;
                case 3: k1 ^= (ulong)data[tail + 2] << 16; goto case 2;
                case 2: k1 ^= (ulong)data[tail + 1] << 8; goto case 1;
                case 1: k1 ^= (ulong)data[tail]; k1 *= c1; k1 = RotateLeft(k1, 31); k1 *= c2; h1 ^= k1;
                    break;
            }

            h1 ^= (ulong)length; h2 ^= (ulong)length;
            h1 += h2; h2 += h1;
            h1 = FMix(h1); h2 = FMix(h2);
            h1 += h2; h2 += h1;

            byte[] result = new byte[16];
            Array.Copy(BitConverter.GetBytes(h1), 0, result, 0, 8);
            Array.Copy(BitConverter.GetBytes(h2), 0, result, 8, 8);
            return result;
        }

        private static ulong RotateLeft(ulong x, byte r) => (x << r) | (x >> (64 - r));
        private static ulong FMix(ulong h) { h ^= h >> 33; h *= 0xff51afd7ed558ccd; h ^= h >> 33; h *= 0xc4ceb9fe1a85ec53; h ^= h >> 33; return h; }

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
