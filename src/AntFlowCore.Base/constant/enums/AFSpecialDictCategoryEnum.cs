namespace AntFlowCore.Base.constant.enums;

/// <summary>
/// Special dictionary category enum.
/// Used to categorize entries in the dictionary (dict_type column).
/// </summary>
public enum AFSpecialDictCategoryEnum
{
    LOWCODEFLOW = 0,
    USER_DEFINED_RULE_FOR_ASSIGNEE = 0
}

public static class AFSpecialDictCategoryEnumExtensions
{
    /// <summary>
    /// The dictionary type string used for low-code flow form codes.
    /// </summary>
    public const string LOWCODEFLOW = "lowcodeflow";

    /// <summary>
    /// The dictionary type string used for user-defined rule assignee options.
    /// </summary>
    public const string USER_DEFINED_RULE_FOR_ASSIGNEE = "udr";

    /// <summary>
    /// The dictionary type string used for process labels.
    /// </summary>
    public const string PROCESSLABEL = "processlabel";

    /// <summary>
    /// 根据字典类型获取汉字含义, 未知类型返回 null(列表原样展示).
    /// </summary>
    public static string GetLabelByDesc(string dictType)
    {
        return dictType switch
        {
            LOWCODEFLOW => "低代码流程",
            USER_DEFINED_RULE_FOR_ASSIGNEE => "自定义审批规则",
            PROCESSLABEL => "流程标签",
            _ => null,
        };
    }

    /// <summary>
    /// 是否为低代码流程类型(系统自动写入, 禁止手动编辑/删除).
    /// </summary>
    public static bool IsLowCodeFlow(string dictType)
    {
        return LOWCODEFLOW == dictType;
    }
}