namespace antflowcore.conf.json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringToIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (int.TryParse(reader.GetString(), out int result))
            {
                return result;
            }
            throw new JsonException($"Invalid integer value: {reader.GetString()}");
        }
        else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
        {
            return number;
        }
        throw new JsonException($"Unexpected token parsing int. Expected String or Number, got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}