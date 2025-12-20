using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

/// <summary>
/// 不论输入是什么类型，都强制转为字符串表示
/// </summary>
public class CoerciveStringConverter : JsonConverter<string>
{
    /// <summary>
    /// Read
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt64(out var i) ? i.ToString() : reader.GetDouble().ToString(),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Null => null,
            JsonTokenType.StartArray or JsonTokenType.StartObject =>
                GetJsonAsString(ref reader),
            _ => throw new JsonException($"Unexpected token: {reader.TokenType}")
        };
    }

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }

    /// <summary>
    /// 读取整个 JSON 对象/数组并返回其原始 JSON 字符串
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static string GetJsonAsString(ref Utf8JsonReader reader)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.GetRawText();
    }
}

