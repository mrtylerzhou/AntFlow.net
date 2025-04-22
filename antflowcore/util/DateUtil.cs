namespace antflowcore.util;

using System;
using System.Globalization;

public static class DateUtil
{
    // Constants for date formats
    public static readonly string YEAR_PATTERN = "yyyy";
    public static readonly string MONTH_PATTERN = "yyyy-MM";
    public static readonly string DATE_PATTERN = "yyyy-MM-dd";
    public static readonly string DATETIME_PATTERN_NO_SECOND = "yyyy-MM-dd HH:mm";
    public static readonly string DATETIME_PATTERN = "yyyy-MM-dd HH:mm:ss";
    public static readonly string DATETIME_PATTERN_WITH_DOT_FULL = "yyyy-MM-dd HH:mm:ss.fff";
    public static readonly string DATETIME_T_PATTERN_NO_SECOND = "yyyy-MM-dd'T'HH:mm";
    public static readonly string DATETIME_T_PATTERN = "yyyy-MM-dd'T'HH:mm:ss";
    public static readonly string DATETIME_T_PATTERN_WITH_DOT_FULL = "yyyy-MM-dd'T'HH:mm:ss.fff";
    public static readonly string DATETIME_PATTERN_COMPRESS = "yyyyMMddHHmmss";
    public static readonly string DATETIME_PATTERN_DATE_COMPRESS = "yyyyMMdd";
    public static readonly string DATETIME_PATTERN_TIME_COMPRESS = "HHmm";
    public static readonly string DATETIME_PATTERN_TIME_SECOND = "HH:mm:ss";
    public static readonly string DATETIME_PATTERN_TIME_NOT_SECOND = "MM-dd";
    public static readonly string DATETIME_PATTERN_TIME_MIN_NOT_SECOND = "HH:mm";
    public static readonly string DATETIME_PATTERN_CHINA = "yyyy年MM月dd日";
    public static readonly string MONTH_PATTERN_CHINA = "yyyy年MM月";
    public static readonly string DATETIME_PATTERN_HOUR = "HH";
    public static readonly string DATETIME_PATTERN_ISO8601 = "yyyy-MM-dd'T'HH:mm:sszzz";

    // Methods for getting start/end dates of day, month, year, etc.

    public static DateTime GetDayStart(DateTime date)
    {
        return date.Date;
    }

    public static DateTime GetYearStart(int year)
    {
        return new DateTime(year, 1, 1);
    }

    public static DateTime GetDayEnd(DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    public static DateTime GetYearEnd(int year)
    {
        return new DateTime(year, 12, 31, 23, 59, 59, 999);
    }

    public static DateTime GetMonthStart(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime GetMonthEnd(DateTime date)
    {
        var lastDay = DateTime.DaysInMonth(date.Year, date.Month);
        return new DateTime(date.Year, date.Month, lastDay, 23, 59, 59, 999);
    }

    public static string GetLastMonth(DateTime date)
    {
        var lastMonth = date.AddMonths(-1);
        return lastMonth.ToString(MONTH_PATTERN);
    }

    public static DayOfWeek GetDayOfWeek(DateTime date)
    {
        return date.DayOfWeek;
    }

    public static int GetYear(DateTime date)
    {
        return date.Year;
    }

    // Add seconds, minutes, hours, days, months, years

    public static DateTime AddSeconds(DateTime date, int seconds)
    {
        return date.AddSeconds(seconds);
    }

    public static DateTime AddMinutes(DateTime date, int minutes)
    {
        return date.AddMinutes(minutes);
    }

    public static DateTime AddHours(DateTime date, int hours)
    {
        return date.AddHours(hours);
    }

    public static DateTime AddDays(DateTime date, int days)
    {
        return date.AddDays(days);
    }

    public static DateTime AddMonths(DateTime date, int months)
    {
        return date.AddMonths(months);
    }

    public static DateTime AddYears(DateTime date, int years)
    {
        return date.AddYears(years);
    }

    // Date difference methods

    public static string DateDiff(DateTime date1, DateTime date2)
    {
        var diff = Math.Abs((date2 - date1).TotalSeconds);
        var days = (int)(diff / (24 * 60 * 60));
        var hours = (int)((diff / (60 * 60)) - days * 24);
        var minutes = (int)((diff / 60) - days * 24 * 60 - hours * 60);
        var seconds = (int)(diff - days * 24 * 60 * 60 - hours * 60 * 60 - minutes * 60);

        return $"{(days > 0 ? days + "天" : "")}{(hours > 0 || days > 0 ? hours + "小时" : "")}{(minutes > 0 || hours > 0 || days > 0 ? minutes + "分" : "")}{seconds}秒";
    }

    public static long DateDiff(DateTime date1, DateTime date2, int mark)
    {
        var diff = Math.Abs((date2 - date1).TotalSeconds);
        var days = (int)(diff / (24 * 60 * 60));
        var hours = (int)((diff / (60 * 60)) - days * 24);
        var minutes = (int)((diff / 60) - days * 24 * 60 - hours * 60);
        var seconds = (int)(diff - days * 24 * 60 * 60 - hours * 60 * 60 - minutes * 60);

        switch (mark)
        {
            case 1: return days;
            case 2: return hours;
            case 3: return minutes;
            case 4: return seconds;
            default: return 0;
        }
    }

    public static DateTime ParseDate(string date)
    {
        return ParseWithPattern(date, DATE_PATTERN);
    }

    // Parse a standard date string (example: "yyyy-MM-dd HH:mm:ss")
    public static DateTime ParseStandard(string dt)
    {
        return ParseWithPattern(dt, DATETIME_PATTERN);
    }

    private static DateTime ParseWithPattern(string dt,string pattern)
    {
        if (DateTime.TryParseExact(dt, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }
        throw new FormatException("Invalid date format.");
    }

    // Day of the week in Chinese
    public static string DayForWeek(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => "周日",
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
            _ => ""
        };
    }

    // Get the season for a given date
    public static int GetSeason(DateTime date)
    {
        int month = date.Month;
        return month switch
        {
            <= 3 => 1,  // Jan-Mar => Spring
            <= 6 => 2,  // Apr-Jun => Summer
            <= 9 => 3,  // Jul-Sep => Autumn
            _ => 4      // Oct-Dec => Winter
        };
    }
}
