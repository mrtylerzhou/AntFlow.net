namespace antflowcore.constant.enums;

using System.Collections.Generic;

/// <summary>
/// Process Disagree Type
/// </summary>
public class ProcessDisagreeTypeEnum
{
    public int Code { get; }
    public string Description { get; }

    private ProcessDisagreeTypeEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public static readonly ProcessDisagreeTypeEnum ONE_DISAGREE = new ProcessDisagreeTypeEnum(1, "退回上一个节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum TWO_DISAGREE = new ProcessDisagreeTypeEnum(2, "退回发起人提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum THREE_DISAGREE = new ProcessDisagreeTypeEnum(3, "退回发起人提交回退节点"); // Default behavior
    public static readonly ProcessDisagreeTypeEnum FOUR_DISAGREE = new ProcessDisagreeTypeEnum(4, "退回历史节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum FIVE_DISAGREE = new ProcessDisagreeTypeEnum(5, "退回历史节点提交回退节点");

    private static readonly List<ProcessDisagreeTypeEnum> Values = new List<ProcessDisagreeTypeEnum>
        {
            ONE_DISAGREE,
            TWO_DISAGREE,
            THREE_DISAGREE,
            FOUR_DISAGREE,
            FIVE_DISAGREE
        };

    /// <summary>
    /// Get description by code
    /// </summary>
    /// <param name="code">The code</param>
    /// <returns>Description of the code or null if not found</returns>
    public static string GetDescriptionByCode(int code)
    {
        foreach (var item in Values)
        {
            if (item.Code == code)
            {
                return item.Description;
            }
        }
        return null;
    }

    /// <summary>
    /// Get enum instance by code
    /// </summary>
    /// <param name="code">The code</param>
    /// <returns>Enum instance or null if not found</returns>
    public static ProcessDisagreeTypeEnum GetByCode(int code)
    {
        foreach (var item in Values)
        {
            if (item.Code == code)
            {
                return item;
            }
        }
        return null;
    }
}