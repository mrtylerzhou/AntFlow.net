namespace AntFlow.Core.Constant.Enums;

/// <summary>
///     Process Disagree Type
/// </summary>
public class ProcessDisagreeTypeEnum
{
    public static readonly ProcessDisagreeTypeEnum ONE_DISAGREE = new(1, "�˻���һ���ڵ��ύ��һ���ڵ�");
    public static readonly ProcessDisagreeTypeEnum TWO_DISAGREE = new(2, "�˻ط������ύ��һ���ڵ�");
    public static readonly ProcessDisagreeTypeEnum THREE_DISAGREE = new(3, "�˻ط������ύ���˽ڵ�"); // Default behavior
    public static readonly ProcessDisagreeTypeEnum FOUR_DISAGREE = new(4, "�˻���ʷ�ڵ��ύ��һ���ڵ�");
    public static readonly ProcessDisagreeTypeEnum FIVE_DISAGREE = new(5, "�˻���ʷ�ڵ��ύ���˽ڵ�");


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