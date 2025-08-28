namespace AntFlow.Core.Constant.Enums;

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
                return "��ǩ��������������ͬ�⣬����˳��";
            case SignTypeEnum.SIGN_TYPE_OR_SIGN:
                return "��ǩ��ֻ��һ��������ͬ���ܾ����ɣ�";
            case SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER:
                return "˳���ǩ(��Ҫ����������ͬ��,����ǰ�˴����˳��)";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static int GetCode(this SignTypeEnum signType)
    {
        return (int)signType;
    }
}