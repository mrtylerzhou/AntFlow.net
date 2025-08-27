namespace AntFlow.Core.Constant.Enums;

public class LFFieldTypeEnum : EnumBase<LFFieldTypeEnum>
{
    public static readonly LFFieldTypeEnum STRING = new(1, "�ַ���");
    public static readonly LFFieldTypeEnum NUMBER = new(2, "����");
    public static readonly LFFieldTypeEnum DATE = new(3, "����");
    public static readonly LFFieldTypeEnum DATE_TIME = new(4, "����ʱ��");
    public static readonly LFFieldTypeEnum TEXT = new(5, "���ַ���");
    public static readonly LFFieldTypeEnum BOOLEAN = new(6, "����");
    public static readonly LFFieldTypeEnum BLOB = new(7, "������");

    private LFFieldTypeEnum(int type, string description) : base(type, description) { }
}