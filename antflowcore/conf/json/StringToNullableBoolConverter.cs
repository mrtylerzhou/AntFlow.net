using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

public class StringToNullableBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True)
            return true;

        if (reader.TokenType == JsonTokenType.False)
            return false;

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (bool.TryParse(str, out var result))
                return result;
            return null;
        }

        if (reader.TokenType == JsonTokenType.Null)
            return null;

        throw new JsonException("Invalid token type for boolean value.");
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}