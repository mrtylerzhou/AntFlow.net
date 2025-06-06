﻿using Microsoft.AspNetCore.Http;

namespace antflowcore.util.Extension;

public static partial class Extensions
{
    public static bool IsEmpty(this object value)
    {
        if (value != null && !string.IsNullOrEmpty(value.ParseToString()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool IsNullOrZero(this object value)
    {
        if (value == null || value.ParseToString().Trim() == "0")
        {
            return true;
        }
        else
        {
            return false;
        }
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