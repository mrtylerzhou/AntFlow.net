using System.Text.Json;
using System.Text.Json.Nodes;

namespace antflowcore.util;

public class JsonNodeHelper
{
    public static JsonNode? SafeParse(string json)
    {
        var cleanOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
            // 不添加全局 converters
        };

        return JsonSerializer.Deserialize<JsonNode>(json, cleanOptions);
    }
}