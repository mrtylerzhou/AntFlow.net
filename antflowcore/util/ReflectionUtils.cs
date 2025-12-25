namespace antflowcore.util;

using System.Reflection;

public static class ReflectionUtils
{
    /// <summary>
    /// 根据属性名获取当前类及其所有父类中的属性值（子类优先）
    /// </summary>
    public static object? GetPropertyValue(object target, string propertyName)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Property name cannot be empty.", nameof(propertyName));
        }

        Type? type = target.GetType();

        while (type != null)
        {
            var property = type.GetProperty(
                propertyName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly |
                BindingFlags.IgnoreCase
            );

            if (property != null)
            {
                return property.GetValue(target);
            }

            type = type.BaseType;
        }

        return null;
    }
}
