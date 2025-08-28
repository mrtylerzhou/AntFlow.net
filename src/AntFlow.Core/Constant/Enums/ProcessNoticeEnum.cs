namespace AntFlow.Core.Constant.Enums;

public class ProcessNoticeEnum
{
    public static readonly ProcessNoticeEnum EMAIL_TYPE = new(1, "�ʼ�");
    public static readonly ProcessNoticeEnum PHONE_TYPE = new(2, "����");
    public static readonly ProcessNoticeEnum APP_TYPE = new(3, "app����");
    public static readonly ProcessNoticeEnum WECHAT_TYPE = new(5, "��΢");
    public static readonly ProcessNoticeEnum DING_TALK_TYPE = new(6, "����");
    public static readonly ProcessNoticeEnum FEISHU_TYPE = new(7, "����");

    private static readonly List<ProcessNoticeEnum> _values = new()
    {
        EMAIL_TYPE,
        PHONE_TYPE,
        APP_TYPE,
        WECHAT_TYPE,
        DING_TALK_TYPE,
        FEISHU_TYPE
    };

    private ProcessNoticeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; }
    public string Desc { get; }

    public static IReadOnlyList<ProcessNoticeEnum> Values => _values;

    public static string GetDescByCode(int code)
    {
        return _values.FirstOrDefault(x => x.Code == code)?.Desc;
    }
}