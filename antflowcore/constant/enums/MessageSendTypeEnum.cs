namespace antflowcore.constant.enums;

public class MessageSendTypeEnum
{
    public int Code { get; }
    public string Description { get; }

    private MessageSendTypeEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public static readonly MessageSendTypeEnum ALL = new MessageSendTypeEnum(50, "所有类型");
    public static readonly MessageSendTypeEnum MAIL = new MessageSendTypeEnum(1, "邮件");
    public static readonly MessageSendTypeEnum MESSAGE = new MessageSendTypeEnum(2, "短信");
    public static readonly MessageSendTypeEnum PUSH = new MessageSendTypeEnum(3, "APP-PUSH");
    public static readonly MessageSendTypeEnum WECHAT_PUSH = new MessageSendTypeEnum(5, "企微消息");
    public static readonly MessageSendTypeEnum DING_TALK_TYPE = new MessageSendTypeEnum(6, "钉钉消息");
    public static readonly MessageSendTypeEnum FEISHU_TYPE = new MessageSendTypeEnum(7, "飞书通知");

    private static readonly Dictionary<int, MessageSendTypeEnum> _codeMap = new Dictionary<int, MessageSendTypeEnum>
    {
        { ALL.Code, ALL },
        { MAIL.Code, MAIL },
        { MESSAGE.Code, MESSAGE },
        { PUSH.Code, PUSH },
        { WECHAT_PUSH.Code, WECHAT_PUSH },
        {DING_TALK_TYPE.Code,DING_TALK_TYPE},
        {FEISHU_TYPE.Code,FEISHU_TYPE}
    };

    public static MessageSendTypeEnum GetEnumByCode(int code)
    {
        _codeMap.TryGetValue(code, out var result);
        return result;
    }
}