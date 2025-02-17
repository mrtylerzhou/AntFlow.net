using System.Collections;

namespace antflowcore.util;

public static class ObjectUtils
{
    public static bool IsEmpty(object obj)
    {
        if (obj == null)
        {
            return true;
        }
        else if (obj is string str)
        {
            return string.IsNullOrEmpty(str);
        }
        else if (obj is IEnumerable enumerable)
        {
            // Check if it's an empty collection, array, or any other enumerable
            foreach (var _ in enumerable)
            {
                return false; // If there's at least one item, it's not empty
            }
            return true;
        }
        else if (obj.GetType().IsArray)
        {
            return ((Array)obj).Length == 0;
        }
        else if (obj is IDictionary dictionary)
        {
            return dictionary.Count == 0;
        }
       
        return false;
    }
}