namespace AntFlow.Core.Constant.Enums;

public class WildcardCharacterEnum
{
    public static readonly WildcardCharacterEnum ONE_CHARACTER = new(1, "{工作流名称}", "BpmnName", false, "\\{工作流名称\\}");
    public static readonly WildcardCharacterEnum TWO_CHARACTER = new(2, "{流程编号}", "ProcessNumber", false, "\\{流程编号\\}");
    public static readonly WildcardCharacterEnum THREE_CHARACTER = new(3, "{申请人}", "StartUser", true, "\\{申请人\\}");

    public static readonly WildcardCharacterEnum
        FOUR_CHARACTER = new(4, "{被审批人}", "ApprovalEmplId", true, "\\{被审批人\\}");

    public static readonly WildcardCharacterEnum FIVE_CHARACTER =
        new(5, "{申请日期}(年月日)", "ApplyDate", false, "\\{申请日期\\}\\(年月日\\)");

    public static readonly WildcardCharacterEnum SIX_CHARACTER =
        new(6, "{申请时间}(年月日时分秒)", "ApplyTime", false, "\\{申请时间\\}\\(年月日时分秒\\)");

    public static readonly WildcardCharacterEnum SEVEN_CHARACTER =
        new(7, "{流转对象}(下一节点审批人)", "NextNodeApproveds", true, "\\{流转对象\\}\\(下一节点审批人\\)");

    public static readonly WildcardCharacterEnum EIGHT_CHARACTER = new(8, "{当前审批人}", "Assignee", true, "\\{当前审批人\\}");
    public static readonly WildcardCharacterEnum NINE_CHARACTER = new(9, "{转发对象}", "ForwardUsers", true, "\\{转发对象\\}");

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