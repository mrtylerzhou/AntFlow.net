using System.Collections;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http;

namespace antflowcore.util.Extension;

public static partial class Extensions
{
    public static bool IsEmpty(this object value)
    {
        if (value == null)
        {
            return true;
        }
        if (value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                return false;
            }
            return true;
        }
        if (value is string s &&!string.IsNullOrEmpty(s))
        {
            return false;
        }

        return false;
    }

    public static bool IsNullOrZero(this object value)
    {
        if (value == null || value.IsZero())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsZero(this object value)
    {
        if (value == null)
        {
            return false;
        }
        return value switch
        {
            // 数字类型...
            byte b => b == 0,
            sbyte sb => sb == 0,
            short s => s == 0,
            ushort us => us == 0,
            int i => i == 0,
            uint ui => ui == 0,
            long l => l == 0,
            ulong ul => ul == 0,
            float f => f == 0f,
            double d => d == 0.0,
            decimal m => m == 0m,
            object obj when decimal.TryParse(obj.ToString(), out var num)=>num==0m,
            _=>false
        };
    }
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException("request");

        if (request.Headers != null)
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        return false;
    }
}