namespace antflowcore.util.Extension;

public static partial class Extension
{
    public static bool IsNumericType(this object obj)
    {
        if (obj == null) return false;

        var type = obj.GetType();

        // 处理可空类型（如 int?, double?）
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        return type switch
        {
            _ when type == typeof(byte) => true,
            _ when type == typeof(sbyte) => true,
            _ when type == typeof(short) => true,
            _ when type == typeof(ushort) => true,
            _ when type == typeof(int) => true,
            _ when type == typeof(uint) => true,
            _ when type == typeof(long) => true,
            _ when type == typeof(ulong) => true,
            _ when type == typeof(float) => true,
            _ when type == typeof(double) => true,
            _ when type == typeof(decimal) => true,
            _ => false
        };
    }
}