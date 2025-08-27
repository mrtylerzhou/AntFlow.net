namespace AntFlow.Core.Constant.Enums;

public class LFControlTypeEnum
{
    public static readonly LFControlTypeEnum SELECT = new(1, "select", "������");

    private LFControlTypeEnum(int code, string name, string desc)
    {
        Code = code;
        Name = name;
        Desc = desc;
    }

    public int Code { get; }
    public string Name { get; }
    public string Desc { get; }

    // ��ѡ�����ڱ���������
    public static IEnumerable<LFControlTypeEnum> Values
    {
        get
        {
            yield return SELECT;
        }
    }
}