namespace antflowcore.constant.enus;

public enum SignTypeEnum
{
    SIGN_TYPE_SIGN = 1,
    SIGN_TYPE_OR_SIGN = 2,
    SIGN_TYPE_SIGN_IN_ORDER = 3
}

public static class SignTypeEnumExtensions
{
    public static string GetDescription(this SignTypeEnum signType)
    {
        switch (signType)
        {
            case SignTypeEnum.SIGN_TYPE_SIGN:
                return "会签（需所有审批人同意，不限顺序）";
            case SignTypeEnum.SIGN_TYPE_OR_SIGN:
                return "或签（只需一名审批人同意或拒绝即可）";
            case SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER:
                return "顺序会签(需要所有审批人同意,根据前端传入的顺序)";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static int GetCode(this SignTypeEnum signType)
    {
        return (int)signType;
    }
}