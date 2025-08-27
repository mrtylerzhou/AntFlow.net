namespace AntFlow.Core.Constant.Enums;

public class WildcardCharacterEnum
{
    public static readonly WildcardCharacterEnum ONE_CHARACTER = new(1, "{����������}", "BpmnName", false, "\\{����������\\}");
    public static readonly WildcardCharacterEnum TWO_CHARACTER = new(2, "{���̱��}", "ProcessNumber", false, "\\{���̱��\\}");
    public static readonly WildcardCharacterEnum THREE_CHARACTER = new(3, "{������}", "StartUser", true, "\\{������\\}");

    public static readonly WildcardCharacterEnum
        FOUR_CHARACTER = new(4, "{��������}", "ApprovalEmplId", true, "\\{��������\\}");

    public static readonly WildcardCharacterEnum FIVE_CHARACTER =
        new(5, "{��������}(������)", "ApplyDate", false, "\\{��������\\}\\(������\\)");

    public static readonly WildcardCharacterEnum SIX_CHARACTER =
        new(6, "{����ʱ��}(������ʱ����)", "ApplyTime", false, "\\{����ʱ��\\}\\(������ʱ����\\)");

    public static readonly WildcardCharacterEnum SEVEN_CHARACTER =
        new(7, "{��ת����}(��һ�ڵ�������)", "NextNodeApproveds", true, "\\{��ת����\\}\\(��һ�ڵ�������\\)");

    public static readonly WildcardCharacterEnum EIGHT_CHARACTER = new(8, "{��ǰ������}", "Assignee", true, "\\{��ǰ������\\}");
    public static readonly WildcardCharacterEnum NINE_CHARACTER = new(9, "{ת������}", "ForwardUsers", true, "\\{ת������\\}");

    private WildcardCharacterEnum(int code, string desc, string filName, bool isSearchEmpl, string transfDesc)
    {
        Code = code;
        Desc = desc;
        FilName = filName;
        IsSearchEmpl = isSearchEmpl;
        TransfDesc = transfDesc;
    }

    public int Code { get; }
    public string Desc { get; }
    public string FilName { get; }
    public bool IsSearchEmpl { get; }
    public string TransfDesc { get; }

    public static IEnumerable<WildcardCharacterEnum> Values
    {
        get
        {
            yield return ONE_CHARACTER;
            yield return TWO_CHARACTER;
            yield return THREE_CHARACTER;
            yield return FOUR_CHARACTER;
            yield return FIVE_CHARACTER;
            yield return SIX_CHARACTER;
            yield return SEVEN_CHARACTER;
            yield return EIGHT_CHARACTER;
            yield return NINE_CHARACTER;
        }
    }
}