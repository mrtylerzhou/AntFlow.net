namespace antflowcore.constant.enus;

public class LFFieldTypeEnum : EnumBase<LFFieldTypeEnum>
{
   
    public static readonly LFFieldTypeEnum STRING = new LFFieldTypeEnum(1, "字符串");
    public static readonly LFFieldTypeEnum NUMBER = new LFFieldTypeEnum(2, "数字");
    public static readonly LFFieldTypeEnum DATE = new LFFieldTypeEnum(3, "日期");
    public static readonly LFFieldTypeEnum DATE_TIME = new LFFieldTypeEnum(4, "日期时间");
    public static readonly LFFieldTypeEnum TEXT = new LFFieldTypeEnum(5, "长字符串");
    public static readonly LFFieldTypeEnum BOOLEAN = new LFFieldTypeEnum(6, "布尔");
    public static readonly LFFieldTypeEnum BLOB = new LFFieldTypeEnum(7, "二进制");

    private LFFieldTypeEnum(int type, string description) : base(type, description) { }
}