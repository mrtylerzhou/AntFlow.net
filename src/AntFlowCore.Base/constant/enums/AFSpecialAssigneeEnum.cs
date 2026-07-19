namespace AntFlowCore.Base.constant.enums;

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
    public static readonly AFSpecialAssigneeEnum CC_NODE = new AFSpecialAssigneeEnum(-1, "-1", "抄送人");
    public static readonly AFSpecialAssigneeEnum SKIP = new AFSpecialAssigneeEnum(-2, "-2", "自动节点自动跳过");
    /// <summary>
    /// 上一节点指定的审批人: 虚拟用户, 运行时由 AFTaskService.InsertTasks 替换为实际审批人
    /// </summary>
    public static readonly AFSpecialAssigneeEnum PREV_NODE_APPOINTED = new AFSpecialAssigneeEnum(-4, "-4", "上一节点指定的审批人");
    public static IEnumerable<AFSpecialAssigneeEnum> Values
    {
        get
        {
            yield return TO_BE_REMOVED;
            yield return COPY_NODE;
            yield return CC_NODE;
            yield return SKIP;
            yield return PREV_NODE_APPOINTED;
        }
    }
}
