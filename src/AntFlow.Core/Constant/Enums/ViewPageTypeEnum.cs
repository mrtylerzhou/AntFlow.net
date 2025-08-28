namespace AntFlow.Core.Constant.Enums;

public enum ViewPageTypeEnum
{
    VIEW_PAGE_TYPE_START = 1, // 发起人
    VIEW_PAGE_TYPE_OTHER = 2 // 其他审批人
}

public static class ViewPageTypeEnumExtensions
{
    public static string GetDescription(this ViewPageTypeEnum viewPageType)
    {
        return viewPageType switch
        {
            ViewPageTypeEnum.VIEW_PAGE_TYPE_START => "发起人",
            ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER => "其他审批人",
            _ => "未知"
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