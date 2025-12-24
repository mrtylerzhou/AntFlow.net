using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

public class StringToLongConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
       
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

      
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();

        
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (long.TryParse(str, out var value))
            {
                return value;
            }

            // 不可解析 → null（不抛异常）
            return null;
        }

      
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var value))
            {
                return value;
            }
            return null;
        }
        
        return null;
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
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
