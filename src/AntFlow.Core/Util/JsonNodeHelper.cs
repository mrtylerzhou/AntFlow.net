using System.Text.Json;
using System.Text.Json.Nodes;

namespace AntFlow.Core.Util;

public class JsonNodeHelper
{
    public static JsonNode? SafeParse(string json)
    {
        JsonSerializerOptions? cleanOptions = new()
        {
            PropertyNameCaseInsensitive = true
            // 自定义转换器 converters
        };

        return JsonSerializer.Deserialize<JsonNode>(json, cleanOptions);
    }
}