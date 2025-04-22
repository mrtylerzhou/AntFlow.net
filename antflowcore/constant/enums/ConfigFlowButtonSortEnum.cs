namespace antflowcore.constant.enus;

using System;
using System.Collections.Generic;
using System.Linq;

public class ConfigFlowButtonSortEnum
{
    public int Code { get; }
    public string Desc { get; }
    public int Sort { get; }

    private ConfigFlowButtonSortEnum(int code, string desc, int sort)
    {
        Code = code;
        Desc = desc;
        Sort = sort;
    }

   
    public static readonly ConfigFlowButtonSortEnum SUBMIT = new ConfigFlowButtonSortEnum(1, "提交", 1);
    public static readonly ConfigFlowButtonSortEnum AGAIN_SUBMIT = new ConfigFlowButtonSortEnum(2, "重新提交", 2);
    public static readonly ConfigFlowButtonSortEnum AGREED = new ConfigFlowButtonSortEnum(3, "同意", 10);
    public static readonly ConfigFlowButtonSortEnum NO_AGREED = new ConfigFlowButtonSortEnum(4, "不同意", 9);
    public static readonly ConfigFlowButtonSortEnum BACK_NODE_EDIT = new ConfigFlowButtonSortEnum(6, "退回上节点修改", 7);
    public static readonly ConfigFlowButtonSortEnum ABANDONED = new ConfigFlowButtonSortEnum(7, "作废", 11);
    public static readonly ConfigFlowButtonSortEnum PRINT = new ConfigFlowButtonSortEnum(8, "打印", 13);
    public static readonly ConfigFlowButtonSortEnum UNDERTAKE = new ConfigFlowButtonSortEnum(10, "承办", 3);
    public static readonly ConfigFlowButtonSortEnum CHANGE_TYPE = new ConfigFlowButtonSortEnum(11, "变更处理人", 4);
    public static readonly ConfigFlowButtonSortEnum END_TYPE = new ConfigFlowButtonSortEnum(12, "终止", 5);
    public static readonly ConfigFlowButtonSortEnum ADD_APPROVAL_PEOPLE = new ConfigFlowButtonSortEnum(13, "添加审批人", 6);
    public static readonly ConfigFlowButtonSortEnum FORWARDING = new ConfigFlowButtonSortEnum(15, "转发", 12);
    public static readonly ConfigFlowButtonSortEnum BACK_EDIT = new ConfigFlowButtonSortEnum(18, "退回修改", 8);
    public static readonly ConfigFlowButtonSortEnum BUTTON_TYPE_JP = new ConfigFlowButtonSortEnum(19, "加批", 19);
    public static readonly ConfigFlowButtonSortEnum SCAN_HELP = new ConfigFlowButtonSortEnum(20, "扫码帮助", 20);
    public static readonly ConfigFlowButtonSortEnum ZB = new ConfigFlowButtonSortEnum(21, "转办", 21);
    public static readonly ConfigFlowButtonSortEnum CHOOSE_ASSIGNEE = new ConfigFlowButtonSortEnum(22, "自选审批人", 22);
    public static readonly ConfigFlowButtonSortEnum BACK_TO_ANY_NODE = new ConfigFlowButtonSortEnum(23, "退回任意节点", 23);

    // Static list of all instances for easy lookup
    private static readonly List<ConfigFlowButtonSortEnum> AllValues = new List<ConfigFlowButtonSortEnum>
    {
        SUBMIT, AGAIN_SUBMIT, AGREED, NO_AGREED, BACK_NODE_EDIT, ABANDONED, PRINT,
        UNDERTAKE, CHANGE_TYPE, END_TYPE, ADD_APPROVAL_PEOPLE, FORWARDING, BACK_EDIT,
        BUTTON_TYPE_JP, SCAN_HELP, ZB, CHOOSE_ASSIGNEE, BACK_TO_ANY_NODE
    };

    public static ConfigFlowButtonSortEnum GetEnumByCode(int? code)
    {
        return AllValues.FirstOrDefault(v => v.Code == code);
    }

    public static string GetDescByCode(int? code)
    {
        var item = GetEnumByCode(code);
        return item != null ? item.Desc : null;
    }

    public static int? GetCodeByDesc(string desc)
    {
        var item = AllValues.FirstOrDefault(v => v.Desc == desc);
        return item != null ? (int?)item.Code : null;
    }
}