namespace antflowcore.constant.enums;

public class AFSpecialAssigneeEnum
{
    public int Code { get; }
    public string Id { get; }
    public string Desc { get; }

    private AFSpecialAssigneeEnum(int code, string id, string desc)
    {
        Code = code;
        Id = id;
        Desc = desc;
    }

    public static readonly AFSpecialAssigneeEnum TO_BE_REMOVED = new AFSpecialAssigneeEnum(0, "0", "最终会被去除的人员");
    public static readonly AFSpecialAssigneeEnum COPY_NODE = new AFSpecialAssigneeEnum(1, "-1", "流程通知");
    public static IEnumerable<AFSpecialAssigneeEnum> Values
    {
        get
        {
            yield return TO_BE_REMOVED;
            yield return COPY_NODE;
        }
    }
}
