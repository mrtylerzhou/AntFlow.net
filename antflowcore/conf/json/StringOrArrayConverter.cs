namespace antflowcore.conf.json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringOrArrayConverter : JsonConverter<string[]>
{
    public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (stringValue == "")
            {
                return Array.Empty<string>();
            }

            throw new JsonException("Expected an array or an empty string.");
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<string[]>(ref reader, options);
        }

        throw new JsonException("Unexpected token type.");
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value);
    }
}