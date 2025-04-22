namespace antflowcore.conf.json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class BooleanToNullableIntJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!.ToLower();
            return value == "true" ? 1 : 0;
        }
        else if (reader.TokenType == JsonTokenType.True)
        {
            return 1;
        }
        else if (reader.TokenType == JsonTokenType.False)
        {
            return 0;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32(); // 直接解析数字
        }

        throw new JsonException("Invalid boolean or number value.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value); // 序列化时输出整数
    }
}