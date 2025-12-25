using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

public class IntToStringListConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<string>();

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return list;

            if (reader.TokenType == JsonTokenType.Number)
            {
                list.Add(reader.GetInt64().ToString());
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                list.Add(reader.GetString()!);
            }
            else
            {
                throw new JsonException($"Unexpected token {reader.TokenType}");
            }
        }

        throw new JsonException("Unexpected end of JSON");
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }
        writer.WriteEndArray();
    }
}
