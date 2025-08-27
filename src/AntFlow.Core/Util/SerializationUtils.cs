using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AntFlow.Core.Util;

public class SerializationUtils
{
    public static T Clone<T>(T obj)
    {
        JsonSerializerOptions? options = new() { TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
        string json = JsonSerializer.Serialize(obj, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }
}