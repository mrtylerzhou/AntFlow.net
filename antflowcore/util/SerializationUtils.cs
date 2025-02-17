using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace antflowcore.util;

public class SerializationUtils
{
    public static T Clone<T>(T obj)
    {
        var options = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
        string json = JsonSerializer.Serialize(obj, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }
}