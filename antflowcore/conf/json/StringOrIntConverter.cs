using antflowcore.exception;

namespace antflowcore.conf.json;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringOrIntConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetDouble().ToString();
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Null:
                return "";
            case JsonTokenType.True:
            case JsonTokenType.False:
                return reader.GetBoolean().ToString();
            case JsonTokenType.StartObject:
            case JsonTokenType.StartArray:
                // 对于对象或数组，可以返回序列化的 JSON 字符串
                var document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.GetRawText();
            default:
                try
                {
                    return reader.GetString();
                }
                catch
                {
                    throw new AFBizException("json处理失败");
                }
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        
        writer.WriteStringValue(value);
    }
}
