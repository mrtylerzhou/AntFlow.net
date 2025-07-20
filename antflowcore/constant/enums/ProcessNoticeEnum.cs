namespace antflowcore.constant.enums;

public class ProcessNoticeEnum
{
    public int Code { get; }
    public string Desc { get; }

    private ProcessNoticeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public static readonly ProcessNoticeEnum EMAIL_TYPE     = new ProcessNoticeEnum(1, "邮件");
    public static readonly ProcessNoticeEnum PHONE_TYPE     = new ProcessNoticeEnum(2, "短信");
    public static readonly ProcessNoticeEnum APP_TYPE       = new ProcessNoticeEnum(3, "app推送");
    public static readonly ProcessNoticeEnum WECHAT_TYPE    = new ProcessNoticeEnum(5, "企微");
    public static readonly ProcessNoticeEnum DING_TALK_TYPE = new ProcessNoticeEnum(6, "钉钉");
    public static readonly ProcessNoticeEnum FEISHU_TYPE    = new ProcessNoticeEnum(7, "飞书");

    private static readonly List<ProcessNoticeEnum> _values = new()
    {
        EMAIL_TYPE,
        PHONE_TYPE,
        APP_TYPE,
        WECHAT_TYPE,
        DING_TALK_TYPE,
        FEISHU_TYPE
    };

    public static IReadOnlyList<ProcessNoticeEnum> Values => _values;

    public static string GetDescByCode(int code)
    {
        return _values.FirstOrDefault(x => x.Code == code)?.Desc;
    }
}
