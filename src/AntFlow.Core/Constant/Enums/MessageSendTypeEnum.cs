namespace AntFlow.Core.Constant.Enums;

public class MessageSendTypeEnum
{
    public static readonly MessageSendTypeEnum ALL = new(50, "��������");
    public static readonly MessageSendTypeEnum MAIL = new(1, "�ʼ�");
    public static readonly MessageSendTypeEnum MESSAGE = new(2, "����");
    public static readonly MessageSendTypeEnum PUSH = new(3, "APP-PUSH");
    public static readonly MessageSendTypeEnum WECHAT_PUSH = new(4, "��΢��Ϣ");

    private static readonly Dictionary<int, MessageSendTypeEnum> _codeMap = new()
    {
        { ALL.Code, ALL },
        { MAIL.Code, MAIL },
        { MESSAGE.Code, MESSAGE },
        { PUSH.Code, PUSH },
        { WECHAT_PUSH.Code, WECHAT_PUSH }
    };

    private MessageSendTypeEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public int Code { get; }
    public string Description { get; }

    public static MessageSendTypeEnum GetEnumByCode(int code)
    {
        _codeMap.TryGetValue(code, out MessageSendTypeEnum? result);
        return result;
    }
}