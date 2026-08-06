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
}
