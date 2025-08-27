namespace AntFlow.Core.Constant.Enums;

public class ApplyType
{
    public static readonly ApplyType ProcessType = new(1, "流程");
    public static readonly ApplyType AppType = new(2, "应用");

    private static readonly List<ApplyType> _all = new() { ProcessType, AppType };

    private ApplyType(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; }
    public string Desc { get; }

    public static string GetDescByCode(int? code)
    {
        if (code == null)
        {
            return null;
        }

        return _all.FirstOrDefault(x => x.Code == code)?.Desc;
    }

    public static int? GetCodeByDesc(string desc)
    {
        if (string.IsNullOrWhiteSpace(desc))
        {
            return null;
        }

        return _all.FirstOrDefault(x => x.Desc == desc)?.Code;
    }

    public static List<ApplyType> GetAll()
    {
        return _all;
    }
}