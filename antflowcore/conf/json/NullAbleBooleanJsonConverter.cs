namespace antflowcore.conf.json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NullAbleBooleanJsonConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!.ToLower();
            return value == "true";
        }
        else if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }
        else if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }

        throw new JsonException("Invalid boolean value.");
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value.HasValue && value.Value);
    }
}