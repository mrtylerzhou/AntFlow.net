namespace AntFlow.Core.Constant.Enums;

public class ConfigFlowButtonSortEnum
{
    public static readonly ConfigFlowButtonSortEnum SUBMIT = new(1, "提交", 1);
    public static readonly ConfigFlowButtonSortEnum AGAIN_SUBMIT = new(2, "重新提交", 2);
    public static readonly ConfigFlowButtonSortEnum AGREED = new(3, "同意", 10);
    public static readonly ConfigFlowButtonSortEnum NO_AGREED = new(4, "不同意", 9);
    public static readonly ConfigFlowButtonSortEnum BACK_NODE_EDIT = new(6, "退回上节点修改", 7);
    public static readonly ConfigFlowButtonSortEnum ABANDONED = new(7, "作废", 11);
    public static readonly ConfigFlowButtonSortEnum PRINT = new(8, "打印", 13);
    public static readonly ConfigFlowButtonSortEnum UNDERTAKE = new(10, "承办", 3);
    public static readonly ConfigFlowButtonSortEnum CHANGE_TYPE = new(11, "变更处理人", 4);
    public static readonly ConfigFlowButtonSortEnum END_TYPE = new(12, "终止", 5);
    public static readonly ConfigFlowButtonSortEnum ADD_APPROVAL_PEOPLE = new(13, "添加审批人", 6);
    public static readonly ConfigFlowButtonSortEnum FORWARDING = new(15, "转发", 12);
    public static readonly ConfigFlowButtonSortEnum BACK_EDIT = new(18, "退回修改", 8);
    public static readonly ConfigFlowButtonSortEnum BUTTON_TYPE_JP = new(19, "加批", 19);
    public static readonly ConfigFlowButtonSortEnum SCAN_HELP = new(20, "扫码帮助", 20);
    public static readonly ConfigFlowButtonSortEnum ZB = new(21, "转办", 21);
    public static readonly ConfigFlowButtonSortEnum CHOOSE_ASSIGNEE = new(22, "自选审批人", 22);
    public static readonly ConfigFlowButtonSortEnum BACK_TO_ANY_NODE = new(23, "退回任意节点", 23);

    // Static list of all instances for easy lookup
    private static readonly List<ConfigFlowButtonSortEnum> AllValues = new()
    {
        SUBMIT,
        AGAIN_SUBMIT,
        AGREED,
        NO_AGREED,
        BACK_NODE_EDIT,
        ABANDONED,
        PRINT,
        UNDERTAKE,
        CHANGE_TYPE,
        END_TYPE,
        ADD_APPROVAL_PEOPLE,
        FORWARDING,
        BACK_EDIT,
        BUTTON_TYPE_JP,
        SCAN_HELP,
        ZB,
        CHOOSE_ASSIGNEE,
        BACK_TO_ANY_NODE
    };

    private ConfigFlowButtonSortEnum(int code, string desc, int sort)
    {
        Code = code;
        Desc = desc;
        Sort = sort;
    }

    public int Code { get; }
    public string Desc { get; }
    public int Sort { get; }

    public static ConfigFlowButtonSortEnum GetEnumByCode(int? code)
    {
        return AllValues.FirstOrDefault(v => v.Code == code);
    }

    public static string GetDescByCode(int? code)
    {
        ConfigFlowButtonSortEnum? item = GetEnumByCode(code);
        return item != null ? item.Desc : null;
    }

    public static int? GetCodeByDesc(string desc)
    {
        ConfigFlowButtonSortEnum? item = AllValues.FirstOrDefault(v => v.Desc == desc);
        return item != null ? item.Code : null;
    }
}