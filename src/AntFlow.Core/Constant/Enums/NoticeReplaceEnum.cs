namespace AntFlow.Core.Constant.Enums;

public class NoticeReplaceEnum
{
    // ==== ö��ֵ���� ====
    public static readonly NoticeReplaceEnum PROCESS_TYPE = new(1, "��������", "ProcessType", false);

    public static readonly NoticeReplaceEnum PROCESS_NAME = new(2, "��������", "ProcessName", false);

    public static readonly NoticeReplaceEnum REJECT_NAME = new(3, "������ͬ����", string.Empty, true);

    public static readonly NoticeReplaceEnum OPERATOR = new(4, "������", string.Empty, true);

    public static readonly NoticeReplaceEnum AFTER_CHANGE_APPROVER = new(5, "���������", string.Empty, true);

    public static readonly NoticeReplaceEnum ORIGINAL_NODE_APPROVER = new(6, "ԭ�����ڵ㴦����", string.Empty, true);

    public static readonly NoticeReplaceEnum PROCESS_ID = new(7, "���̱��", "ProcessId", false);

    private NoticeReplaceEnum(int code, string desc, string filName, bool isSelectEmpl)
    {
        Code = code;
        Desc = desc;
        FilName = filName;
        IsSelectEmpl = isSelectEmpl;
    }

    public int Code { get; }
    public string Desc { get; }
    public string FilName { get; }
    public bool IsSelectEmpl { get; }

    // ==== ����ֵ�б���������� ====
    public static IEnumerable<NoticeReplaceEnum> Values
    {
        get
        {
            yield return PROCESS_TYPE;
            yield return PROCESS_NAME;
            yield return REJECT_NAME;
            yield return OPERATOR;
            yield return AFTER_CHANGE_APPROVER;
            yield return ORIGINAL_NODE_APPROVER;
            yield return PROCESS_ID;
        }
    }

    // ==== ���� ====
    public static string GetDescByCode(int code)
    {
        return Values.FirstOrDefault(e => e.Code == code)?.Desc;
    }

    public override string ToString()
    {
        return Desc;
    }
}