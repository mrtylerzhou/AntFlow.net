namespace AntFlow.Core.Constant.Enums;

public enum DeduplicationTypeEnum
{
    DEDUPLICATION_TYPE_NULL = 1,
    DEDUPLICATION_TYPE_FORWARD = 2,
    DEDUPLICATION_TYPE_BACKWARD = 3
}

public static class DeduplicationTypeEnumExtensions
{
    public static string GetDesc(this DeduplicationTypeEnum enums)
    {
        switch (enums)
        {
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_NULL:
                return "��ȥ��";
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_FORWARD:
                return "��һ���������ظ�����ʱ��ֻ�����һ��������ǰȥ�أ�";
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_BACKWARD:
                return "��һ���������ظ�����ʱ��ֻ�ڵ�һ����������ȥ�أ�";
            default:
                return null;
        }
    }

    public static string GetDescByCode(int code)
    {
        if (Enum.IsDefined(typeof(DeduplicationTypeEnum), code))
        {
            DeduplicationTypeEnum enums = (DeduplicationTypeEnum)code;
            return enums.GetDesc();
        }

        return null;
    }
}