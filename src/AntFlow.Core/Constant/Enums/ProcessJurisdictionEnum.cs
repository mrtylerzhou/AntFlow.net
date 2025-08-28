namespace AntFlow.Core.Constant.Enums;

public class ProcessJurisdictionEnum
{
    public static readonly ProcessJurisdictionEnum VIEW_TYPE = new(1, "查看");
    public static readonly ProcessJurisdictionEnum CREATE_TYPE = new(2, "创建");
    public static readonly ProcessJurisdictionEnum CONTROL_TYPE = new(3, "控制");

    // 用于按 code 查找枚举
    private static readonly Dictionary<int, ProcessJurisdictionEnum> CodeMap = new()
    {
        { VIEW_TYPE.Code, VIEW_TYPE }, { CREATE_TYPE.Code, CREATE_TYPE }, { CONTROL_TYPE.Code, CONTROL_TYPE }
    };

    private ProcessJurisdictionEnum(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public int Code { get; }
    public string Description { get; }

    // 可选：返回所有枚举值，类似 Java 的 values()
    public static IEnumerable<ProcessJurisdictionEnum> Values => CodeMap.Values;

    public static string GetDescByCode(int code)
    {
        return CodeMap.TryGetValue(code, out ProcessJurisdictionEnum? enumValue)
            ? enumValue.Description
            : null;
    }
}