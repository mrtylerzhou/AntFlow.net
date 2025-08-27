namespace AntFlow.Core.Constant.Enums;

public class LFControlTypeEnum
{
    public static readonly LFControlTypeEnum SELECT = new(1, "select", "下拉框");

    private LFControlTypeEnum(int code, string name, string desc)
    {
        Code = code;
        Name = name;
        Desc = desc;
    }

    public int Code { get; }
    public string Name { get; }
    public string Desc { get; }

    // 可选：用于遍历所有项
    public static IEnumerable<LFControlTypeEnum> Values
    {
        get
        {
            yield return SELECT;
        }
    }
}