namespace antflowcore.constant.enus;

public class ApplyType
{
    public int Code { get; private set; }
    public string Desc { get; private set; }

    private ApplyType(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public static readonly ApplyType ProcessType = new ApplyType(1, "流程");
    public static readonly ApplyType AppType = new ApplyType(2, "应用");

    private static readonly List<ApplyType> _all = new List<ApplyType> { ProcessType, AppType };

    public static string GetDescByCode(int? code)
    {
        if (code == null) return null;
        return _all.FirstOrDefault(x => x.Code == code)?.Desc;
    }

    public static int? GetCodeByDesc(string desc)
    {
        if (string.IsNullOrWhiteSpace(desc)) return null;
        return _all.FirstOrDefault(x => x.Desc == desc)?.Code;
    }

    public static List<ApplyType> GetAll() => _all;
}
