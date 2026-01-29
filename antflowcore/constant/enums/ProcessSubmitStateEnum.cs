namespace antflowcore.constant.enus;
public enum ProcessSubmitStateEnum
{
    /// <summary>
    /// 流程提交状态
    /// </summary>
    PROCESS_SUB_TYPE = 1,

    /// <summary>
    /// 流程同意状态
    /// </summary>
    PROCESS_AGRESS_TYPE = 2,

    /// <summary>
    /// 流程不同意状态
    /// </summary>
    PROCESS_NO_AGRESS_TYPE = 3,

    /// <summary>
    /// 流程撤回状态
    /// </summary>
    WITHDRAW_AGRESS_TYPE = 4,

    /// <summary>
    /// 流程作废状态
    /// </summary>
    END_AGRESS_TYPE = 5,

    /// <summary>
    /// 流程终止状态
    /// </summary>
    CRMCEL_AGRESS_TYPE = 6,

    /// <summary>
    /// 流程撤回状态（back）
    /// </summary>
    WITHDRAW_DISAGREE_TYPE = 7,

    /// <summary>
    /// 打回修改
    /// </summary>
    PROCESS_UPDATE_TYPE = 8,

    /// <summary>
    /// 加批
    /// </summary>
    PROCESS_SIGN_UP = 9,
    /// <summary>
    /// 转交
    /// </summary>
    PROCESS_TRANSFER_ASSIGNEE = 10
}

public static class ProcessSubmitStateEnumExtensions
{
    /// <summary>
    /// 获取描述信息
    /// </summary>
    private static readonly (ProcessSubmitStateEnum Code, string Desc)[] Descriptions = new[]
    {
            (ProcessSubmitStateEnum.PROCESS_SUB_TYPE, "流程提交状态"),
            (ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE, "流程同意状态"),
            (ProcessSubmitStateEnum.PROCESS_NO_AGRESS_TYPE, "流程不同意状态"),
            (ProcessSubmitStateEnum.WITHDRAW_AGRESS_TYPE, "流程撤回状态"),
            (ProcessSubmitStateEnum.END_AGRESS_TYPE, "流程作废状态"),
            (ProcessSubmitStateEnum.CRMCEL_AGRESS_TYPE, "流程终止状态"),
            (ProcessSubmitStateEnum.WITHDRAW_DISAGREE_TYPE, "back"),
            (ProcessSubmitStateEnum.PROCESS_UPDATE_TYPE, "打回修改"),
            (ProcessSubmitStateEnum.PROCESS_SIGN_UP, "加批"),
            (ProcessSubmitStateEnum.PROCESS_TRANSFER_ASSIGNEE, "转交")
        };

    /// <summary>
    /// 根据代码获取描述
    /// </summary>
    public static string GetDesc(this ProcessSubmitStateEnum code)
    {
        return Descriptions.FirstOrDefault(d => d.Code == code).Desc ?? string.Empty;
    }

    /// <summary>
    /// 根据代码值获取描述
    /// </summary>
    public static string GetDescByCode(int code)
    {
        return Descriptions.FirstOrDefault(d => (int)d.Code == code).Desc ?? null;
    }
}