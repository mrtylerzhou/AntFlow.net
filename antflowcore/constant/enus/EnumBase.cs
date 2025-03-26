using System.Reflection;

namespace antflowcore.constant.enus;

public abstract class EnumBase<T> where T : EnumBase<T>
{
    public int Type { get; }
    public string Description { get; }

    protected EnumBase(int type, string description)
    {
        Type = type;
        Description = description;
        if (_values.ContainsKey(this.GetType().FullName))
        {
            _values[this.GetType().FullName].Add((T)this);
        }
        else
        {
            List<T> lst=new List<T>();
            lst.Add((T)this);
            _values.Add(this.GetType().FullName, lst);
        }
    }

    private static readonly Dictionary<string,List<T>> _values = new Dictionary<string, List<T>>();

    public static IReadOnlyDictionary<string,List<T>> Values => _values.AsReadOnly();

    public static T? GetByType(int type) => _values[typeof(T).FullName].FirstOrDefault(e => e.Type == type);

    public override string ToString() =>Type+ Description;
    
    public override bool Equals(object? obj)
    {
        if (obj is EnumBase<T> other)
        {
            return Type == other.Type;
        }
        return false;
    }
    public override int GetHashCode() => Type.GetHashCode();
    
    public static bool operator ==(EnumBase<T>? left, EnumBase<T>? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Type == right.Type;
    }

    public static bool operator !=(EnumBase<T>? left, EnumBase<T>? right) => !(left == right);
    public static void InitializeEnumBaseTypes()
    {
        // 获取当前程序集中的所有类型
        var assembly = Assembly.GetExecutingAssembly();
        var enumBaseTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(EnumBase<>)) && !t.IsAbstract);

        foreach (var type in enumBaseTypes)
        {
            // 强制访问该类型的静态字段，确保它们被初始化
            var staticField = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(field => field.IsStatic);

            if (staticField != null)
            {
                // 访问静态字段来初始化它
                var value = staticField.GetValue(null);
            }
        }
    }
}