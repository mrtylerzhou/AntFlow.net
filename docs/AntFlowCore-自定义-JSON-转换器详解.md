# AntFlowCore 自定义 JSON 转换器详解：解决 AspNetCore 默认序列化的严格问题

## 前言

AntFlowCore 坚持最小依赖原则，只使用 ASP.NET Core 自带的 `System.Text.Json` 做序列化，不引入第三方的 Newtonsoft.Json。但是 `System.Text.Json` 默认序列化规则太严格，在实际开发中会碰到很多兼容性问题。

AntFlowCore 自定义了一系列 JSON 转换器解决这些常见问题，本文详细介绍每个转换器解决了什么问题，怎么工作的。

## 为什么需要自定义转换器

ASP.NET Core 默认 `System.Text.Json` 有这些痛点：

1. **布尔 vs 整数**：数据库一般用 `int(1)` 存储布尔（1=true，0=false），但默认序列化不兼容布尔字符串转整数
2. **字符串 vs 数字ID**：前端传ID有时候用字符串，有时候用数字，默认严格校验不兼容
3. **可空类型**：可空布尔/整数处理不够灵活，null 值处理不好
4. **日期时间**：默认格式不一定符合前端需求
5. **DateOnly**：.NET 6+ 新增的 DateOnly，默认不支持序列化

AntFlowCore 针对这些痛点，一个一个解决了。

## 布尔转整数：BooleanToIntJsonConverter

### 问题背景

数据库设计中，我们经常用 `int(1)` 存储布尔值：
- `1` 表示 `true`
- `0` 表示 `false`

但是 C# 实体属性定义成 `int`，前端可能传 JSON 布尔值 `true/false`，默认 `System.Text.Json` 不能直接把布尔转成 int，会抛出异常。

### 解决代码

```csharp
public class BooleanToIntJsonConverter : JsonConverter<int>
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
            return 1; // JSON 布尔 true → int 1
        }
        else if (reader.TokenType == JsonTokenType.False)
        {
            return 0; // JSON 布尔 false → int 0
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32(); // 本来就是数字，直接读
        }

        throw new JsonException("Invalid boolean or number value.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value); // 序列化输出整数
    }
}
```

### 支持多种输入格式

| 前端输入 | 转换结果 |
|---------|---------|
| `true` | `1` |
| `false` | `0` |
| `"true"` | `1` |
| `"false"` | `0` |
| `1` | `1` |
| `0` | `0` |

不管前端传布尔还是数字，都能正确解析，非常灵活。

### 可空版本：BooleanToNullableIntJsonConverter

如果属性是可空的 `int?`，使用这个版本：

```csharp
public class BooleanToNullableIntJsonConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null; // null 直接返回 null
        }
        // ... 其他逻辑和上面一样
    }
}
```

处理可空布尔到可空 int，逻辑一致，只是支持 null。

## 布尔转换器：BooleanJsonConverter

### 问题背景

有时候 C# 属性就是 `bool` 类型，但是前端可能传 `"true"` 字符串，或者 `1`/`0` 数字，默认 `System.Text.Json` 不兼容这些格式。

### 解决代码

```csharp
public class BooleanJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!.ToLower().Trim();
            // 支持字符串 "true"/"false"
            return value == "true" || value == "1";
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            // 支持数字 1/0
            return reader.GetInt32() != 0;
        }
        return reader.GetBoolean();
    }
}
```

### 支持多种输入

| 输入 | 结果 |
|------|------|
| `true` | `true` |
| `false` | `false` |
| `"true"` | `true` |
| `"1"` | `true` |
| `1` | `true` |
| `0` | `false` |

比默认灵活很多，不管前端怎么传，都能正确解析。

## 可空布尔：NullAbleBooleanJsonConverter

对应可空布尔版本：

```csharp
public class NullAbleBooleanJsonConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        // ... 逻辑和 BooleanJsonConverter 一样，只是支持 null
    }
}
```

## 字符串转整数：StringToIntConverter

### 问题背景

ID 字段在 C# 定义成 `int`/`long`，前端有时候传数字，有时候因为 JS 精度问题传字符串（比如 bigint），默认 `System.Text.Json` 不允许字符串转数字，直接报错。

### 解决代码

```csharp
public class StringToIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string str = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            return int.Parse(str); // 字符串 → int
        }
        return reader.GetInt32(); // 本来就是数字，直接读
    }
}
```

不管前端传数字还是字符串，都能正确转换成 int，解决了 JS 精度问题。

### 字符串转长整型：StringToLongConverter

同理，针对 `long` 类型：

```csharp
public class StringToLongConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string str = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            return long.Parse(str);
        }
        return reader.GetInt64();
    }
}
```

解决了 JS 处理 64-bit 整数精度丢失问题，前端可以用字符串传，后端正确转成 long。

## 字符串或整数：StringOrIntConverter

### 问题背景

AntFlowCore 里面 ID 可能存字符串也可能存数字，这个转换器兼容两种格式：

```csharp
public class StringOrIntConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return string.IsNullOrEmpty(str) ? 0 : long.Parse(str);
        }
        return reader.GetInt64();
    }
}
```

不管前端传字符串还是数字，都能转成 `long`，非常灵活。

## 全局可空 int：GlobalNullableIntConverter

### 问题背景

前端传过来 `""` 空字符串，想转换成 `null` 给可空 int，默认 `System.Text.Json` 不能自动转换。

### 解决代码

```csharp
public class GlobalNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            string str = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(str))
            {
                return null; // 空字符串 → null
            }
            return int.Parse(str);
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        return null;
    }
}
```

这个转换器处理了：
- `null` → `null`
- 空字符串 `""` → `null`
- 字符串数字 → 解析成 int
- 数字 → 直接读

非常友好，前端各种空值情况都处理好了。

## 强制字符串：CoerciveStringConverter

### 问题背景

不管前端传什么类型，都转成字符串：数字 → 字符串，bool → 字符串，null → 空字符串。

### 代码

```csharp
public class CoerciveStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => string.Empty,
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Number => reader.GetDecimal().ToString(CultureInfo.InvariantCulture),
            _ => string.Empty
        };
    }
}
```

使用场景：数据库字段是字符串，前端可能传各种类型，强制转成字符串存储，不会报错。

## 字符串转布尔：StringToNullableBoolConverter

### 问题背景

可空布尔，前端可能传字符串 `"true"/"false"`，默认不兼容：

```csharp
public class StringToNullableBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }
        if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32() != 0;
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!;
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            value = value.ToLower();
            return value == "true" || value == "1";
        }
        return null;
    }
}
```

支持 `null`/空字符串 → `null`，字符串 `"true"`/`"1"` → `true`，完美解决问题。

## 日期时间：CustomDateTimeConverter

### 问题背景

默认 `System.Text.Json` 序列化 DateTime 格式不一定符合前端需求，我们统一格式化：

```csharp
public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
```

统一输出 `yyyy-MM-dd HH:mm:ss` 格式，前端不需要再转换，直接能用。

## DateOnly 支持：DateOnlyConverter

.NET 6 引入了 `DateOnly` 类型，`System.Text.Json` 默认低版本不支持序列化，AntFlowCore 自己实现：

```csharp
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString()!);
    }
}
```

输出格式 `yyyy-MM-dd`，符合日常使用。

## 数组或字符串转字符串数组：StringOrArrayConverter

### 问题背景

有时候前端传单个字符串，有时候传数组，想要统一转成 `List<string>`：

```csharp
public class StringOrArrayConverter : JsonConverter<List<string>>
{
    public override List<string> Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // 单个字符串，分割成数组
            var str = reader.GetString()!;
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }
            return str.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // 已经是数组，直接读
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
                list.Add(reader.GetString()!);
            }
            return list;
        }
        return new List<string>();
    }
}
```

非常实用：
- 前端传 `"a,b,c"` → `List<string>{"a", "b", "c"}`
- 前端传 `["a", "b", "c"]` → 同样 `List<string>`
- 不管前端怎么传，后端都能正确解析

## 整数转字符串：IntToStringConverter

为什么需要把 int 转成字符串输出？前端 JS 处理 long 会有精度问题，所以 ID 输出给前端用字符串，避免精度丢失。

```csharp
public class IntToStringConverter : JsonConverter<int>
{
    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
```

int 序列化成字符串，前端 JS 可以安全处理不会丢精度。

## 整数列表转字符串列表：IntToStringListConverter

同理，List<int> 序列化成 List<string>:

```csharp
public class IntToStringListConverter : JsonConverter<List<int>>
{
    // 每个 int 都转成字符串输出，避免 JS 精度问题
}
```

## 注册所有转换器

AntFlowCore 在 `Program.cs` 统一注册所有转换器：

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new BooleanToIntJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new BooleanToNullableIntJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullAbleBooleanJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new StringToIntConverter());
        options.JsonSerializerOptions.Converters.Add(new StringToLongConverter());
        options.JsonSerializerOptions.Converters.Add(new StringOrIntConverter());
        options.JsonSerializerOptions.Converters.Add(new GlobalNullableIntConverter());
        options.JsonSerializerOptions.Converters.Add(new CoerciveStringConverter());
        options.JsonSerializerOptions.Converters.Add(new StringToNullableBoolConverter());
        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new StringOrArrayConverter());
        options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
        options.JsonSerializerOptions.Converters.Add(new IntToStringListConverter());
    });
```

就是这么简单，全部注册好了，整个应用都能享受灵活的序列化处理。

## 总结

AntFlowCore 自定义这些转换器，就是为了解决 `System.Text.Json` 默认规则太严格的问题：

| 问题 | 转换器 | 解决方法 |
|------|--------|---------|
| 布尔 JSON → int 数据库 | `BooleanToIntJsonConverter` | 兼容布尔/数字/字符串，自动转换 |
| 字符串 ID → 数字类型 | `StringToIntConverter` / `StringToLongConverter` | 兼容字符串/数字，解决 JS 精度问题 |
| 空字符串 → 可空 null | `GlobalNullableIntConverter` | 空字符串自动转换成 null |
| 多种输入格式转布尔 | `BooleanJsonConverter` / `StringToNullableBoolConverter` | 布尔/数字/字符串都能正确转 |
| 日期格式统一 | `CustomDateTimeConverter` / `DateOnlyConverter` | 统一输出格式，前端直接用 |
| 单个字符串/数组 → 列表 | `StringOrArrayConverter` | 兼容两种输入格式 |
| 整数 → 字符串输出 | `IntToStringConverter` | 避免 JS 精度丢失 |

通过这些自定义转换器，AntFlowCore 在保持最小依赖（只使用自带 `System.Text.Json`）的同时，解决了实际开发中绝大多数兼容性问题，让前端和后端对接更加顺畅。

---

**相关链接：**
- [System.Text.Json 官方文档](https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/custom-converters)
- [上一篇：低代码表单设计详解](./AntFlowCore-低代码表单设计详解.md)
