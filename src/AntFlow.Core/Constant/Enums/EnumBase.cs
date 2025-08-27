using System.Reflection;

namespace AntFlow.Core.Constant.Enums;

public abstract class EnumBase<T> where T : EnumBase<T>
{
    private static readonly Dictionary<string, List<T>> _values = new();

    protected EnumBase(int type, string description)
    {
        Type = type;
        Description = description;
        if (_values.ContainsKey(GetType().FullName))
        {
            _values[GetType().FullName].Add((T)this);
        }
        else
        {
            List<T> lst = new();
            lst.Add((T)this);
            _values.Add(GetType().FullName, lst);
        }
    }

    public int Type { get; }
    public string Description { get; }

    public static IReadOnlyDictionary<string, List<T>> Values => _values.AsReadOnly();

    public static T? GetByType(int type)
    {
        return _values[typeof(T).FullName].FirstOrDefault(e => e.Type == type);
    }

    public override string ToString()
    {
        return Type + Description;
    }

    public override bool Equals(object? obj)
    {
        if (obj is EnumBase<T> other)
        {
            return Type == other.Type;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    public static bool operator ==(EnumBase<T>? left, EnumBase<T>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Type == right.Type;
    }

    public static bool operator !=(EnumBase<T>? left, EnumBase<T>? right)
    {
        return !(left == right);
    }

    public static void InitializeEnumBaseTypes()
    {
        Assembly? assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> enumBaseTypes = assembly.GetTypes()
            .Where(t => t.BaseType != null &&
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(EnumBase<>));

        foreach (Type? type in enumBaseTypes)
        {
            FieldInfo? staticField = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(field => field.IsStatic);

            if (staticField != null)
            {
                object? value = staticField.GetValue(null);
            }
        }
    }
}