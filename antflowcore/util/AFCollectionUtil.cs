namespace antflowcore.util;

/// <summary>
/// A convenient collection utility class mainly used to convert ID types.
/// </summary>
public static class AFCollectionUtil
{
    public static IEnumerable<string> NumberToStringList<T>(IEnumerable<T> numbers) where T : struct, IConvertible
    {
        return numbers.Select(num => num.ToString());
    }

    public static IEnumerable<string> SerializeToStringCollection(IEnumerable<object> collection)
    {
        return collection.Select(obj => obj.ToString());
    }

    public static IEnumerable<int> StringToIntList(IEnumerable<string> numbers)
    {
        return numbers.Select(int.Parse);
    }

    public static IEnumerable<long> StringToLongList(IEnumerable<string> numbers)
    {
        return numbers.Select(long.Parse);
    }
    

    public static IEnumerable<int> LongToIntList(IEnumerable<long> numbers)
    {
        return numbers.Select(num => (int)num);
    }
}