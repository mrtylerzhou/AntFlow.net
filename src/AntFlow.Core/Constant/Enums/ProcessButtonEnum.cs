namespace AntFlow.Core.Constant.Enums;

public class ProcessButtonEnum
{
    // 定义所有类型实例
    public static readonly ProcessButtonEnum DISAGREE_TYPE = new(1, "不同意");
    public static readonly ProcessButtonEnum AGREE_TYPE = new(2, "同意");
    public static readonly ProcessButtonEnum WITHDRAW_TYPE = new(3, "撤回");
    public static readonly ProcessButtonEnum END_TYPE = new(4, "终止");
    public static readonly ProcessButtonEnum DELETE_TYPE = new(5, "作废");
    public static readonly ProcessButtonEnum CHANGE_TYPE = new(6, "变更处理人");
    public static readonly ProcessButtonEnum HANDLE_TYPE = new(7, "代审批");
    public static readonly ProcessButtonEnum PRINTRING_TYPE = new(8, "打印");
    public static readonly ProcessButtonEnum CEO_TYPE = new(9, "提交CEO审批");
    public static readonly ProcessButtonEnum FORWARD_TYPE = new(10, "转发");
    public static readonly ProcessButtonEnum ACTING_TYPE = new(11, "代审批不同意");
    public static readonly ProcessButtonEnum UNDERTAKE_TYPE = new(12, "承办");
    public static readonly ProcessButtonEnum JOINTLY_SIGN = new(14, "会签");
    public static readonly ProcessButtonEnum GET_BACK = new(15, "返回");
    public static readonly ProcessButtonEnum ADD_BATCH = new(16, "增加审批人");
    public static readonly ProcessButtonEnum STAFF_CONFIRM_TYPE = new(17, "代员工确认");
    public static readonly ProcessButtonEnum VIEW_TYPE = new(1, "查看类型");
    public static readonly ProcessButtonEnum DEAL_WITH_TYPE = new(2, "处理类型");
    public static readonly ProcessButtonEnum MAIN_COLOR = new(1, "primary");
    public static readonly ProcessButtonEnum DEFAULT_COLOR = new(2, "default");

    // 所有类型集合（用于查询）
    private static readonly List<ProcessButtonEnum> AllTypes = new()
    {
        DISAGREE_TYPE,
        AGREE_TYPE,
        WITHDRAW_TYPE,
        END_TYPE,
        DELETE_TYPE,
        CHANGE_TYPE,
        HANDLE_TYPE,
        PRINTRING_TYPE,
        CEO_TYPE,
        FORWARD_TYPE,
        ACTING_TYPE,
        UNDERTAKE_TYPE,
        JOINTLY_SIGN,
        GET_BACK,
        ADD_BATCH,
        STAFF_CONFIRM_TYPE,
        VIEW_TYPE,
        DEAL_WITH_TYPE,
        MAIN_COLOR,
        DEFAULT_COLOR
    };

    // 私有字段

    // 构造函数
    private ProcessButtonEnum(int code, string desc)
    {
        this.Code = code;
        this.Desc = desc;
    }

    // 属性访问器
    public int Code { get; }

    public string Desc { get; }

    // 通过Code获取Desc
    public static string GetDescByCode(int code)
    {
        return AllTypes.FirstOrDefault(t => t.Code == code)?.Desc;
    }

    // 通过Desc获取Code
    public static int? GetCodeByDesc(string desc)
    {
        return AllTypes.FirstOrDefault(t => t.Desc == desc)?.Code;
    }
}