namespace AntFlow.Core.Constant.Enums;

public class NoticeReplaceEnum
{
    // ==== 枚举值定义 ====
    public static readonly NoticeReplaceEnum PROCESS_TYPE = new(1, "流程类型", "ProcessType", false);

    public static readonly NoticeReplaceEnum PROCESS_NAME = new(2, "流程名称", "ProcessName", false);

    public static readonly NoticeReplaceEnum REJECT_NAME = new(3, "审批不同意者", string.Empty, true);

    public static readonly NoticeReplaceEnum OPERATOR = new(4, "操作者", string.Empty, true);

    public static readonly NoticeReplaceEnum AFTER_CHANGE_APPROVER = new(5, "变更后处理人", string.Empty, true);

    public static readonly NoticeReplaceEnum ORIGINAL_NODE_APPROVER = new(6, "原审批节点处理人", string.Empty, true);

    public static readonly NoticeReplaceEnum PROCESS_ID = new(7, "流程编号", "ProcessId", false);

    private NoticeReplaceEnum(int code, string desc, string filName, bool isSelectEmpl)
    {
        Code = code;
        Desc = desc;
        FilName = filName;
        IsSelectEmpl = isSelectEmpl;
    }

    public int Code { get; }
    public string Desc { get; }
    public string FilName { get; }
    public bool IsSelectEmpl { get; }

    // ==== 所有值列表（方便遍历） ====
    public static IEnumerable<NoticeReplaceEnum> Values
    {
        get
        {
            yield return PROCESS_TYPE;
            yield return PROCESS_NAME;
            yield return REJECT_NAME;
            yield return OPERATOR;
            yield return AFTER_CHANGE_APPROVER;
            yield return ORIGINAL_NODE_APPROVER;
            yield return PROCESS_ID;
        }
    }

    // ==== 方法 ====
    public static string GetDescByCode(int code)
    {
        return Values.FirstOrDefault(e => e.Code == code)?.Desc;
    }

    public override string ToString()
    {
        return Desc;
    }
}