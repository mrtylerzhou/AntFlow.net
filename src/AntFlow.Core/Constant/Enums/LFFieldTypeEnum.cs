namespace AntFlow.Core.Constant.Enums;

public class LFFieldTypeEnum : EnumBase<LFFieldTypeEnum>
{
    public static readonly LFFieldTypeEnum STRING = new(1, "字符串");
    public static readonly LFFieldTypeEnum NUMBER = new(2, "数字");
    public static readonly LFFieldTypeEnum DATE = new(3, "日期");
    public static readonly LFFieldTypeEnum DATE_TIME = new(4, "日期时间");
    public static readonly LFFieldTypeEnum TEXT = new(5, "长字符串");
    public static readonly LFFieldTypeEnum BOOLEAN = new(6, "布尔");
    public static readonly LFFieldTypeEnum BLOB = new(7, "二进制");

    private LFFieldTypeEnum(int type, string description) : base(type, description) { }
}