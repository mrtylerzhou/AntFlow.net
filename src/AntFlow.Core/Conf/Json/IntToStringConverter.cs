using System.Text.Json;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Conf.Json;

public class IntToStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64().ToString();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        throw new JsonException($"Unexpected token parsing int. Expected String or Number, got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}