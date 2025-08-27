namespace AntFlow.Core.Constant.Enums;

public enum ViewPageTypeEnum
{
    VIEW_PAGE_TYPE_START = 1, // ������
    VIEW_PAGE_TYPE_OTHER = 2 // ����������
}

public static class ViewPageTypeEnumExtensions
{
    public static string GetDescription(this ViewPageTypeEnum viewPageType)
    {
        return viewPageType switch
        {
            ViewPageTypeEnum.VIEW_PAGE_TYPE_START => "������",
            ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER => "����������",
            _ => "δ֪"
        };
    }

    public static int GetCode(this ViewPageTypeEnum viewPageType)
    {
        return (int)viewPageType;
    }

    public static ViewPageTypeEnum? GetByCode(int code)
    {
        return Enum.IsDefined(typeof(ViewPageTypeEnum), code)
            ? (ViewPageTypeEnum?)code
            : null;
    }
}