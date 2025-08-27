using System.Collections;

namespace AntFlow.Core.Util;

public static class ObjectUtils
{
    public static bool IsEmpty(object obj)
    {
        if (obj == null)
        {
            return true;
        }

        if (obj is string str)
        {
            return string.IsNullOrEmpty(str);
        }

        if (obj is IEnumerable enumerable)
        {
            // Check if it's an empty collection, array, or any other enumerable
            foreach (object? _ in enumerable)
            {
                return false; // If there's at least one item, it's not empty
            }

            return true;
        }

        if (obj.GetType().IsArray)
        {
            return ((Array)obj).Length == 0;
        }

        if (obj is IDictionary dictionary)
        {
            return dictionary.Count == 0;
        }

        return false;
    }
}