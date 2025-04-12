namespace antflowcore.constant.enus;

public class ProcessTypeEnum
{
    public int Code { get; }
    public string Desc { get; }

    private ProcessTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public static readonly ProcessTypeEnum VIEW_TYPE      = new ProcessTypeEnum(1,  "查看流程");
    public static readonly ProcessTypeEnum MONITOR_TYPE   = new ProcessTypeEnum(2,  "监控流程");
    public static readonly ProcessTypeEnum LAUNCH_TYPE    = new ProcessTypeEnum(3,  "新建流程");
    public static readonly ProcessTypeEnum PARTIC_TYPE    = new ProcessTypeEnum(4,  "已办流程");
    public static readonly ProcessTypeEnum ENTRUST_TYPE   = new ProcessTypeEnum(5,  "代办流程");
    public static readonly ProcessTypeEnum DRAFT_TYPE     = new ProcessTypeEnum(6,  "草稿流程");
    public static readonly ProcessTypeEnum ADMIN_TYPE     = new ProcessTypeEnum(8,  "流程管理");
    public static readonly ProcessTypeEnum ALL_TYPE       = new ProcessTypeEnum(10, "APP查询代办，新建，已办流程");

    private static readonly List<ProcessTypeEnum> All = new()
    {
        VIEW_TYPE, MONITOR_TYPE, LAUNCH_TYPE, PARTIC_TYPE,
        ENTRUST_TYPE, DRAFT_TYPE, ADMIN_TYPE, ALL_TYPE
    };

    public static string? GetDescByCode(int code)
    {
        return All.FirstOrDefault(x => x.Code == code)?.Desc;
    }

    public static ProcessTypeEnum? FromCode(int code)
    {
        return All.FirstOrDefault(x => x.Code == code);
    }

    public static IReadOnlyList<ProcessTypeEnum> List() => All.AsReadOnly();
}
