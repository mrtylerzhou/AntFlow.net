﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntFlowCore.Util
{
    public static class JiMuCommonUtils
    {
        /// <summary>
        /// Converts a list of strings to a comma-separated string.
        /// </summary>
        public static string ListToString(List<string> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }

            list.Sort();
            return string.Join(",", list);
        }

        /// <summary>
        /// Converts a comma-separated string to a list of strings.
        /// </summary>
        public static List<string> StrToList(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return str.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        /// <summary>
        /// Converts an exception stack trace to a string.
        /// </summary>
        public static string ExceptionToString(Exception e)
        {
            if (e == null)
            {
                return string.Empty;
            }

            // Using the built-in Exception.ToString() method to get the stack trace
            return e.ToString(); // C# has a built-in method to format exceptions
        }
    }
}