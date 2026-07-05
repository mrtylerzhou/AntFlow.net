namespace AntFlowCore.Base.constant.enums;

public enum DeduplicationTypeEnum
{
    DEDUPLICATION_TYPE_NULL = 1,
    DEDUPLICATION_TYPE_FORWARD = 2,
    DEDUPLICATION_TYPE_BACKWARD = 3,
    DEDUPLICATION_TYPE_SKIP_NEXT = 4
}

public static class DeduplicationTypeEnumExtensions
{
    public static string GetDesc(this DeduplicationTypeEnum enums)
    {
        switch (enums)
        {
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_NULL:
                return "不去重";
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_FORWARD:
                return "当一个审批人重复出现时，只在最后一次审批（前去重）";
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_BACKWARD:
                return "当一个审批人重复出现时，只在第一次审批（后去重）";
            case DeduplicationTypeEnum.DEDUPLICATION_TYPE_SKIP_NEXT:
                return "当一个审批人仅在相邻节点重复出现时,后续自动同意";
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