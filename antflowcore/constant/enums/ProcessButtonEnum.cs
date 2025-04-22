namespace antflowcore.constant.enums;

using System.Collections.Generic;
using System.Linq;

public class ProcessButtonEnum
{
    // 定义所有类型实例
    public static readonly ProcessButtonEnum DISAGREE_TYPE = new ProcessButtonEnum(1, "不同意");

    public static readonly ProcessButtonEnum AGREE_TYPE = new ProcessButtonEnum(2, "同意");
    public static readonly ProcessButtonEnum WITHDRAW_TYPE = new ProcessButtonEnum(3, "撤回");
    public static readonly ProcessButtonEnum END_TYPE = new ProcessButtonEnum(4, "终止");
    public static readonly ProcessButtonEnum DELETE_TYPE = new ProcessButtonEnum(5, "作废");
    public static readonly ProcessButtonEnum CHANGE_TYPE = new ProcessButtonEnum(6, "变更处理人");
    public static readonly ProcessButtonEnum HANDLE_TYPE = new ProcessButtonEnum(7, "代审批");
    public static readonly ProcessButtonEnum PRINTRING_TYPE = new ProcessButtonEnum(8, "打印");
    public static readonly ProcessButtonEnum CEO_TYPE = new ProcessButtonEnum(9, "提交CEO审批");
    public static readonly ProcessButtonEnum FORWARD_TYPE = new ProcessButtonEnum(10, "转发");
    public static readonly ProcessButtonEnum ACTING_TYPE = new ProcessButtonEnum(11, "代审批不同意");
    public static readonly ProcessButtonEnum UNDERTAKE_TYPE = new ProcessButtonEnum(12, "承办");
    public static readonly ProcessButtonEnum JOINTLY_SIGN = new ProcessButtonEnum(14, "会签");
    public static readonly ProcessButtonEnum GET_BACK = new ProcessButtonEnum(15, "返回");
    public static readonly ProcessButtonEnum ADD_BATCH = new ProcessButtonEnum(16, "增加审批人");
    public static readonly ProcessButtonEnum STAFF_CONFIRM_TYPE = new ProcessButtonEnum(17, "代员工确认");
    public static readonly ProcessButtonEnum VIEW_TYPE = new ProcessButtonEnum(1, "查看类型");
    public static readonly ProcessButtonEnum DEAL_WITH_TYPE = new ProcessButtonEnum(2, "处理类型");
    public static readonly ProcessButtonEnum MAIN_COLOR = new ProcessButtonEnum(1, "primary");
    public static readonly ProcessButtonEnum DEFAULT_COLOR = new ProcessButtonEnum(2, "default");

    // 私有字段
    private readonly int code;

    private readonly string desc;

    // 构造函数
    private ProcessButtonEnum(int code, string desc)
    {
        this.code = code;
        this.desc = desc;
    }

    // 属性访问器
    public int Code => code;

    public string Desc => desc;

    // 所有类型集合（用于查询）
    private static readonly List<ProcessButtonEnum> AllTypes = new List<ProcessButtonEnum>
    {
        DISAGREE_TYPE, AGREE_TYPE, WITHDRAW_TYPE, END_TYPE, DELETE_TYPE,
        CHANGE_TYPE, HANDLE_TYPE, PRINTRING_TYPE, CEO_TYPE, FORWARD_TYPE,
        ACTING_TYPE, UNDERTAKE_TYPE, JOINTLY_SIGN, GET_BACK, ADD_BATCH,
        STAFF_CONFIRM_TYPE, VIEW_TYPE, DEAL_WITH_TYPE, MAIN_COLOR, DEFAULT_COLOR
    };

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