namespace AntFlow.Core.Constant.Enums;

public enum VariantFormContainerTypeEnum
{
    CARD, // ��Ƭ
    TAB, // ��ǩҳ
    TABLE, // ���
    GRID // դ��
}

public static class VariantFormContainerTypeEnumExtensions
{
    private static readonly Dictionary<VariantFormContainerTypeEnum, (string TypeName, string Description)>
        EnumDetails =
            new()
            {
                { VariantFormContainerTypeEnum.CARD, ("card", "��Ƭ") },
                { VariantFormContainerTypeEnum.TAB, ("tab", "��ǩҳ") },
                { VariantFormContainerTypeEnum.TABLE, ("table", "���") },
                { VariantFormContainerTypeEnum.GRID, ("grid", "դ��") }
            };

    /// <summary>
    ///     ��ȡö��ֵ��Ӧ���������ơ�
    /// </summary>
    public static string GetTypeName(this VariantFormContainerTypeEnum enumValue)
    {
        return EnumDetails[enumValue].TypeName;
    }

    /// <summary>
    ///     ��ȡö��ֵ��������Ϣ��
    /// </summary>
    public static string GetDescription(this VariantFormContainerTypeEnum enumValue)
    {
        return EnumDetails[enumValue].Description;
    }

    /// <summary>
    ///     �����������ƻ�ȡ��Ӧ��ö��ֵ��
    ///     ���δ�ҵ����򷵻� null��
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
    ///     �����������ƻ�ȡ��Ӧ��ö��ֵ��
    ///     ���δ�ҵ������׳��쳣��
    /// </summary>
    public static VariantFormContainerTypeEnum GetByTypeNameOrThrow(string typeName)
    {
        VariantFormContainerTypeEnum? result = GetByTypeName(typeName);
        if (result == null)
        {
            throw new ArgumentException($"Invalid typeName: {typeName}", nameof(typeName));
        }

        return result.Value;
    }
}