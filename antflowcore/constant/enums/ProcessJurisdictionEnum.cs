namespace antflowcore.constant.enums;

public class ProcessJurisdictionEnum
{
    public int Code { get; }
    public string Description { get; }

    private ProcessJurisdictionEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }
    
    public static readonly ProcessJurisdictionEnum VIEW_TYPE = new(1, "查看");
    public static readonly ProcessJurisdictionEnum CREATE_TYPE = new(2, "创建");
    public static readonly ProcessJurisdictionEnum CONTROL_TYPE = new(3, "监控");

    // 用于按 code 查找描述
    private static readonly Dictionary<int, ProcessJurisdictionEnum> CodeMap = new()
    {
        { VIEW_TYPE.Code, VIEW_TYPE },
        { CREATE_TYPE.Code, CREATE_TYPE },
        { CONTROL_TYPE.Code, CONTROL_TYPE }
    };

    public static string GetDescByCode(int code)
    {
        return CodeMap.TryGetValue(code, out var enumValue)
            ? enumValue.Description
            : null;
    }

    // 可选：返回所有枚举项（类似 Java 的 values()）
    public static IEnumerable<ProcessJurisdictionEnum> Values => CodeMap.Values;
}