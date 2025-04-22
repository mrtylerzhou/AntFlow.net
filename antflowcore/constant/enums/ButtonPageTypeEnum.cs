namespace antflowcore.constant.enums;

public enum ButtonPageTypeEnum
{
    INITIATE = 1, // 发起页
    AUDIT = 2,    // 审批页
    TOVIEW = 3    // 查看页
}

public static class ButtonPageTypeEnumExtensions
{
    private static readonly Dictionary<ButtonPageTypeEnum, (string Name, string Desc)> EnumMetadata =
        new Dictionary<ButtonPageTypeEnum, (string Name, string Desc)>
        {
            { ButtonPageTypeEnum.INITIATE, ("initiate", "发起页") },
            { ButtonPageTypeEnum.AUDIT, ("audit", "审批页") },
            { ButtonPageTypeEnum.TOVIEW, ("toView", "查看页") }
        };

    public static string GetName(this ButtonPageTypeEnum buttonPageType)
    {
        return EnumMetadata.TryGetValue(buttonPageType, out var metadata) ? metadata.Name : null;
    }

    public static string GetDesc(this ButtonPageTypeEnum buttonPageType)
    {
        return EnumMetadata.TryGetValue(buttonPageType, out var metadata) ? metadata.Desc : null;
    }

    public static ButtonPageTypeEnum? GetEnumByCode(int code)
    {
        return EnumMetadata.Keys.FirstOrDefault(e => (int)e == code);
    }

    public static string GetDescByCode(int code)
    {
        return GetEnumByCode(code)?.GetDesc();
    }

    public static int? GetCodeByDesc(string desc)
    {
        return EnumMetadata.FirstOrDefault(e => e.Value.Desc == desc).Key as int?;
    }
}