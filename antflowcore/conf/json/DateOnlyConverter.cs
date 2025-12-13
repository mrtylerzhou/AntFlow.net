using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace antflowcore.conf.json;

public class DateOnlyConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        // 先按完整 ISO 解析
        if (DateTime.TryParse(str, out var full))
            return full;
        // 再按纯日期解析
        if (DateTime.TryParseExact(str, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateOnly))
            return dateOnly;
        throw new JsonException($"无法将 [{str}] 反序列化为 DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        // 序列化时统一输出 ISO 完整格式
        writer.WriteStringValue(value?.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}