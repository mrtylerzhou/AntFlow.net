namespace antflowcore.constant.enus;

public class WildcardCharacterEnum
{
    public int Code { get; }
    public string Desc { get; }
    public string FilName { get; }
    public bool IsSearchEmpl { get; }
    public string TransfDesc { get; }

    private WildcardCharacterEnum(int code, string desc, string filName, bool isSearchEmpl, string transfDesc)
    {
        this.Code = code;
        this.Desc = desc;
        this.FilName = filName;
        this.IsSearchEmpl = isSearchEmpl;
        this.TransfDesc = transfDesc;
    }

    public static readonly WildcardCharacterEnum ONE_CHARACTER = new WildcardCharacterEnum(1, "{工作流名称}", "BpmnName", false, "\\{工作流名称\\}");
    public static readonly WildcardCharacterEnum TWO_CHARACTER = new WildcardCharacterEnum(2, "{流程编号}", "ProcessNumber", false, "\\{流程编号\\}");
    public static readonly WildcardCharacterEnum THREE_CHARACTER = new WildcardCharacterEnum(3, "{申请人}", "StartUser", true, "\\{申请人\\}");
    public static readonly WildcardCharacterEnum FOUR_CHARACTER = new WildcardCharacterEnum(4, "{被审批人}", "ApprovalEmplId", true, "\\{被审批人\\}");
    public static readonly WildcardCharacterEnum FIVE_CHARACTER = new WildcardCharacterEnum(5, "{申请日期}(年月日)", "ApplyDate", false, "\\{申请日期\\}\\(年月日\\)");
    public static readonly WildcardCharacterEnum SIX_CHARACTER = new WildcardCharacterEnum(6, "{申请时间}(年月日时分秒)", "ApplyTime", false, "\\{申请时间\\}\\(年月日时分秒\\)");
    public static readonly WildcardCharacterEnum SEVEN_CHARACTER = new WildcardCharacterEnum(7, "{流转对象}(下一节点审批人)", "NextNodeApproveds", true, "\\{流转对象\\}\\(下一节点审批人\\)");
    public static readonly WildcardCharacterEnum EIGHT_CHARACTER = new WildcardCharacterEnum(8, "{当前审批人}", "Assignee", true, "\\{当前审批人\\}");
    public static readonly WildcardCharacterEnum NINE_CHARACTER = new WildcardCharacterEnum(9, "{转发对象}", "ForwardUsers", true, "\\{转发对象\\}");

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
