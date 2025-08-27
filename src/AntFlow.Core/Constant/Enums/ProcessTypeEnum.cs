namespace AntFlow.Core.Constant.Enums;

public class ProcessTypeEnum
{
    public static readonly ProcessTypeEnum VIEW_TYPE = new(1, "查看流程");
    public static readonly ProcessTypeEnum MONITOR_TYPE = new(2, "监控流程");
    public static readonly ProcessTypeEnum LAUNCH_TYPE = new(3, "新建流程");
    public static readonly ProcessTypeEnum PARTIC_TYPE = new(4, "已办流程");
    public static readonly ProcessTypeEnum ENTRUST_TYPE = new(5, "委托流程");
    public static readonly ProcessTypeEnum DRAFT_TYPE = new(6, "草稿流程");
    public static readonly ProcessTypeEnum ADMIN_TYPE = new(8, "流程管理");
    public static readonly ProcessTypeEnum ALL_TYPE = new(10, "APP查询所有，新建和已办流程");

    private static readonly List<ProcessTypeEnum> All = new()
    {
        VIEW_TYPE,
        MONITOR_TYPE,
        LAUNCH_TYPE,
        PARTIC_TYPE,
        ENTRUST_TYPE,
        DRAFT_TYPE,
        ADMIN_TYPE,
        ALL_TYPE
    };

    private ProcessTypeEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    public int Code { get; }
    public string Desc { get; }

    public static string? GetDescByCode(int code)
    {
        return All.FirstOrDefault(x => x.Code == code)?.Desc;
    }

    public static ProcessTypeEnum? FromCode(int code)
    {
        return All.FirstOrDefault(x => x.Code == code);
    }

    public static IReadOnlyList<ProcessTypeEnum> List()
    {
        return All.AsReadOnly();
    }
}