using System.Collections;
using System.Globalization;

namespace AntFlow.Core.Util.Extension;

public static partial class Extensions
{
    #region ????Guid

    /// <summary>
    ///     ??string????Guid???????????????Guid.Empty???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Guid ParseToGuid(this string str)
    {
        try
        {
            return new Guid(str);
        }
        catch
        {
            return Guid.Empty;
        }
    }

    #endregion ????Guid

    #region ??????????

    /// <summary>
    ///     ??????????
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> CastSuper<TResult>(this IEnumerable source)
    {
        foreach (object item in source)
        {
            yield return (TResult)Convert.ChangeType(item, typeof(TResult));
        }
    }

    #endregion ??????????

    /// <summary>
    ///     ??DateTime????????????
    /// </summary>
    /// <param name="dateTime">??????DateTime????</param>
    /// <returns>????????????????</returns>
    public static int ConvertToTimestamp(this DateTime dateTime)
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = dateTime.ToUniversalTime() - origin;
        return (int)Math.Floor(diff.TotalSeconds);
    }

    #region ????long

    /// <summary>
    ///     ??object????long???????????????0???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long ParseToLong(this object obj)
    {
        try
        {
            return long.Parse(obj.ToString());
        }
        catch
        {
            return 0L;
        }
    }

    /// <summary>
    ///     ??object????long??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ParseToLong(this string str, long defaultValue)
    {
        try
        {
            return long.Parse(str);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion ????long

    #region ????int

    /// <summary>
    ///     ??object????int???????????????0???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str)
    {
        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    ///     ??object????int??????????????????????????????
    ///     null????????
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ParseToInt(this object str, int defaultValue)
    {
        if (str == null)
        {
            return defaultValue;
        }

        try
        {
            return Convert.ToInt32(str);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion ????int

    #region ????short

    /// <summary>
    ///     ??object????short???????????????0???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short ParseToShort(this object obj)
    {
        try
        {
            return short.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    ///     ??object????short??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short ParseToShort(this object str, short defaultValue)
    {
        try
        {
            return short.Parse(str.ToString());
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion ????short

    #region ????demical

    /// <summary>
    ///     ??object????demical??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str, decimal defaultValue)
    {
        try
        {
            return decimal.Parse(str.ToString());
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     ??object????demical???????????????0???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ParseToDecimal(this object str)
    {
        try
        {
            return decimal.Parse(str.ToString());
        }
        catch
        {
            return 0;
        }
    }

    #endregion ????demical

    #region ????bool

    /// <summary>
    ///     ??object????bool???????????????false???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str)
    {
        try
        {
            return bool.Parse(str.ToString());
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     ??object????bool??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool ParseToBool(this object str, bool result)
    {
        try
        {
            return bool.Parse(str.ToString());
        }
        catch
        {
            return result;
        }
    }

    #endregion ????bool

    #region ????float

    /// <summary>
    ///     ??object????float???????????????0???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str)
    {
        try
        {
            return float.Parse(str.ToString());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    ///     ??object????float??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float ParseToFloat(this object str, float result)
    {
        try
        {
            return float.Parse(str.ToString());
        }
        catch
        {
            return result;
        }
    }

    #endregion ????float

    #region ????DateTime

    /// <summary>
    ///     ??string????DateTime?????????????????????С????????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DateTime.MinValue;
            }

            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }

            int length = str.Length;
            switch (length)
            {
                case 4:
                    return DateTime.ParseExact(str, "yyyy", CultureInfo.CurrentCulture);

                case 6:
                    return DateTime.ParseExact(str, "yyyyMM", CultureInfo.CurrentCulture);

                case 8:
                    return DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.CurrentCulture);

                case 10:
                    return DateTime.ParseExact(str, "yyyyMMddHH", CultureInfo.CurrentCulture);

                case 12:
                    return DateTime.ParseExact(str, "yyyyMMddHHmm", CultureInfo.CurrentCulture);

                case 14:
                    return DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);

                default:
                    return DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
            }
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    ///     ??string????DateTime?????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static DateTime ParseToDateTime(this string str, DateTime? defaultValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue.GetValueOrDefault();
            }

            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }

            int length = str.Length;
            switch (length)
            {
                case 4:
                    return DateTime.ParseExact(str, "yyyy", CultureInfo.CurrentCulture);

                case 6:
                    return DateTime.ParseExact(str, "yyyyMM", CultureInfo.CurrentCulture);

                case 8:
                    return DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.CurrentCulture);

                case 10:
                    return DateTime.ParseExact(str, "yyyyMMddHH", CultureInfo.CurrentCulture);

                case 12:
                    return DateTime.ParseExact(str, "yyyyMMddHHmm", CultureInfo.CurrentCulture);

                case 14:
                    return DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);

                default:
                    return DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
            }
        }
        catch
        {
            return defaultValue.GetValueOrDefault();
        }
    }

    #endregion ????DateTime

    #region ????string

    /// <summary>
    ///     ??object????string???????????????""???????????
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ParseToString(this object obj)
    {
        try
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return obj.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string ParseToStrings<T>(this object obj)
    {
        try
        {
            IEnumerable<T>? list = obj as IEnumerable<T>;
            if (list != null)
            {
                return string.Join(",", list);
            }

            return obj.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion ????string

    #region ????double

    /// <summary>
    ///     ??object????double???????????????0???????????
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object obj)
    {
        try
        {
            return double.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    ///     ??object????double??????????????????????????????
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ParseToDouble(this object str, double defaultValue)
    {
        try
        {
            return double.Parse(str.ToString());
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion ????double
}