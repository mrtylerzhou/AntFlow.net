namespace AntFlow.Core.Constant.Enums;

public class AFSpecialAssigneeEnum
{
    public static readonly AFSpecialAssigneeEnum TO_BE_REMOVED = new(0, "0", "最终会被去除的人员");
    public static readonly AFSpecialAssigneeEnum COPY_NODE = new(1, "-1", "流程通知");

    private AFSpecialAssigneeEnum(int code, string id, string desc)
    {
        Code = code;
        Id = id;
        Desc = desc;
    }

    public int Code { get; }
    public string Id { get; }
    public string Desc { get; }

    public static IEnumerable<AFSpecialAssigneeEnum> Values
    {
        get
        {
            yield return TO_BE_REMOVED;
            yield return COPY_NODE;
        }
    }
}