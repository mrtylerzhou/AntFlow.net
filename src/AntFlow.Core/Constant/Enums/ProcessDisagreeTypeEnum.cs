namespace AntFlow.Core.Constant.Enums;

/// <summary>
///     Process Disagree Type
/// </summary>
public class ProcessDisagreeTypeEnum
{
    public static readonly ProcessDisagreeTypeEnum ONE_DISAGREE = new(1, "退回上一个节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum TWO_DISAGREE = new(2, "退回发起人提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum THREE_DISAGREE = new(3, "退回发起人提交回退节点"); // Default behavior
    public static readonly ProcessDisagreeTypeEnum FOUR_DISAGREE = new(4, "退回历史节点提交下一个节点");
    public static readonly ProcessDisagreeTypeEnum FIVE_DISAGREE = new(5, "退回历史节点提交回退节点");


    private static readonly List<ProcessDisagreeTypeEnum> Values = new()
    {
        ONE_DISAGREE,
        TWO_DISAGREE,
        THREE_DISAGREE,
        FOUR_DISAGREE,
        FIVE_DISAGREE
    };

    private ProcessDisagreeTypeEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public int Code { get; }
    public string Description { get; }

    /// <summary>
    ///     Get description by code
    /// </summary>
    /// <param name="code">The code</param>
    /// <returns>Description of the code or null if not found</returns>
    public static string GetDescriptionByCode(int code)
    {
        foreach (ProcessDisagreeTypeEnum? item in Values)
        {
            if (item.Code == code)
            {
                return item.Description;
            }
        }

        return null;
    }

    /// <summary>
    ///     Get enum instance by code
    /// </summary>
    /// <param name="code">The code</param>
    /// <returns>Enum instance or null if not found</returns>
    public static ProcessDisagreeTypeEnum GetByCode(int code)
    {
        foreach (ProcessDisagreeTypeEnum? item in Values)
        {
            if (item.Code == code)
            {
                return item;
            }
        }

        return null;
    }
}