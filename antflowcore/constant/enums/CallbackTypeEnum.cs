namespace antflowcore.constant.enus;

public enum CallbackTypeEnum
{
    CONF_CONDITION_CALL_BACK,
    PROC_CONDITION_CALL_BACK,
    PROC_SUBMIT_CALL_BACK,
    PROC_STARTED_CALL_BACK,
    PROC_COMMIT_CALL_BACK,
    PROC_END_CALL_BACK,
    PROC_FINISH_CALL_BACK
}
public static class CallbackTypeEnumExtensions
{
    public static string GetMark(this CallbackTypeEnum type)
    {
        return type.ToString(); // mark 就是枚举名
    }

    public static string GetDesc(this CallbackTypeEnum type)
    {
        return type switch
        {
            CallbackTypeEnum.CONF_CONDITION_CALL_BACK => "条件分支回调",
            CallbackTypeEnum.PROC_CONDITION_CALL_BACK => "条件判断回调",
            CallbackTypeEnum.PROC_SUBMIT_CALL_BACK => "提交操作回调",
            CallbackTypeEnum.PROC_STARTED_CALL_BACK => "流程发起完成回调",
            CallbackTypeEnum.PROC_COMMIT_CALL_BACK => "流转操作回调",
            CallbackTypeEnum.PROC_END_CALL_BACK => "结束操作回调",
            CallbackTypeEnum.PROC_FINISH_CALL_BACK => "完成操作回调",
            _ => string.Empty
        };
    }
}
