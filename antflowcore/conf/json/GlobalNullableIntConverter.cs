
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

public class GlobalNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }
            // 如果不是有效数字，返回null而不是抛异常，更灵活
            return null;
        }
        else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
        {
            return number;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
