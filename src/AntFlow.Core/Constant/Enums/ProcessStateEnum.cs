namespace AntFlow.Core.Constant.Enums;

public enum ProcessStateEnum
{
    HANDLING_STATE = 1, // 处理中
    END_STATE = 3, // 结束
    HANDLE_STATE = 2, // 审批通过
    REJECT_STATE = 6 // 审批拒绝
}

public static class ProcessStateEnumExtensions
{
    // 描述信息字典
    private static readonly Dictionary<ProcessStateEnum, string> Descriptions = new()
    {
        { ProcessStateEnum.HANDLING_STATE, "处理中" },
        { ProcessStateEnum.END_STATE, "结束" },
        { ProcessStateEnum.HANDLE_STATE, "审批通过" },
        { ProcessStateEnum.REJECT_STATE, "审批拒绝" }
    };

    // 根据枚举值获取描述
    public static string GetDescription(this ProcessStateEnum state)
    {
        return Descriptions.TryGetValue(state, out string? desc) ? desc : null;
    }

    // 根据描述获取枚举值
    public static ProcessStateEnum? GetByDescription(string desc)
    {
        return Descriptions.FirstOrDefault(kv => kv.Value == desc).Key;
    }

    // 获取所有枚举值
    public static IEnumerable<ProcessStateEnum> GetAllValues()
    {
        return Enum.GetValues(typeof(ProcessStateEnum)).Cast<ProcessStateEnum>();
    }

    // 根据 Code 获取枚举值
    public static ProcessStateEnum? GetByCode(int code)
    {
        return Enum.IsDefined(typeof(ProcessStateEnum), code)
            ? (ProcessStateEnum?)code
            : null;
    }

    public static string GetDescByCode(int code)
    {
        ProcessStateEnum? processStateEnum = Enum.IsDefined(typeof(ProcessStateEnum), code)
            ? (ProcessStateEnum?)code
            : null;
        if (processStateEnum == null)
        {
            return "";
        }

        return GetDescription(processStateEnum.Value);
    }
}