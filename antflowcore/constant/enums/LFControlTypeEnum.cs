namespace antflowcore.constant.enums;

public class LFControlTypeEnum
{
    public int Code { get; }
    public string Name { get; }
    public string Desc { get; }

    private LFControlTypeEnum(int code, string name, string desc)
    {
        Code = code;
        Name = name;
        Desc = desc;
    }

    public static readonly LFControlTypeEnum SELECT = new LFControlTypeEnum(1, "select", "下拉框");

    // 可选：用于遍历所有项
    public static IEnumerable<LFControlTypeEnum> Values
    {
        get
        {
            yield return SELECT;
        }
    }
}
