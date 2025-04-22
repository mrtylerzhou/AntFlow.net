namespace antflowcore.constant.enums;

using System;
using System.Collections.Generic;
using System.Linq;

public enum VariantFormContainerTypeEnum
{
    CARD,    // 卡片
    TAB,     // 标签页
    TABLE,   // 表格
    GRID     // 栅格
}

public static class VariantFormContainerTypeEnumExtensions
{
    private static readonly Dictionary<VariantFormContainerTypeEnum, (string TypeName, string Description)> EnumDetails =
        new Dictionary<VariantFormContainerTypeEnum, (string, string)>
        {
            { VariantFormContainerTypeEnum.CARD, ("card", "卡片") },
            { VariantFormContainerTypeEnum.TAB, ("tab", "标签页") },
            { VariantFormContainerTypeEnum.TABLE, ("table", "表格") },
            { VariantFormContainerTypeEnum.GRID, ("grid", "栅格") }
        };

    /// <summary>
    /// 获取枚举值对应的类型名称。
    /// </summary>
    public static string GetTypeName(this VariantFormContainerTypeEnum enumValue)
    {
        return EnumDetails[enumValue].TypeName;
    }

    /// <summary>
    /// 获取枚举值的描述信息。
    /// </summary>
    public static string GetDescription(this VariantFormContainerTypeEnum enumValue)
    {
        return EnumDetails[enumValue].Description;
    }

    /// <summary>
    /// 根据类型名称获取对应的枚举值。
    /// 如果未找到，则返回 null。
    /// </summary>
    public static VariantFormContainerTypeEnum? GetByTypeName(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        return EnumDetails
            .FirstOrDefault(kv => kv.Value.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            .Key;
    }

    /// <summary>
    /// 根据类型名称获取对应的枚举值。
    /// 如果未找到，则抛出异常。
    /// </summary>
    public static VariantFormContainerTypeEnum GetByTypeNameOrThrow(string typeName)
    {
        var result = GetByTypeName(typeName);
        if (result == null)
        {
            throw new ArgumentException($"Invalid typeName: {typeName}", nameof(typeName));
        }

        return result.Value;
    }
}