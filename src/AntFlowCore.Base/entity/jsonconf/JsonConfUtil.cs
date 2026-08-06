using System.Text.Json;
using System.Text.Json.Serialization;

namespace AntFlowCore.Base.entity.jsonconf;

public static class JsonConfUtil
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string? ToJsonString(object? obj)
    {
        return obj == null ? null : JsonSerializer.Serialize(obj, Options);
    }

    public static T? ParseObject<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static BpmnConfConfigJson? ParseConfConfig(string? json)
    {
        return ParseObject<BpmnConfConfigJson>(json);
    }

    public static string? ToConfConfigJson(BpmnConfConfigJson? config)
    {
        return ToJsonString(config);
    }

    public static BpmnNodeConfigJson? ParseNodeConfig(string? json)
    {
        return ParseObject<BpmnNodeConfigJson>(json);
    }

    public static string? ToNodeConfigJson(BpmnNodeConfigJson? config)
    {
        return ToJsonString(config);
    }

    public static VariableConfigJson? ParseVariableConfig(string? json)
    {
        return ParseObject<VariableConfigJson>(json);
    }

    public static string? ToVariableConfigJson(VariableConfigJson? config)
    {
        return ToJsonString(config);
    }
}
