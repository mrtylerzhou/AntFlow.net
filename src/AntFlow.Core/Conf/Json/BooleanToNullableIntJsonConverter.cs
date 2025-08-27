using System.Text.Json;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Conf.Json;

public class BooleanToNullableIntJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!.ToLower();
            return value == "true" ? 1 : 0;
        }

        if (reader.TokenType == JsonTokenType.True)
        {
            return 1;
        }

        if (reader.TokenType == JsonTokenType.False)
        {
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32(); // ֱ�ӽ�������
        }

        throw new JsonException("Invalid boolean or number value.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value); // 序列化时输出数字
    }
}