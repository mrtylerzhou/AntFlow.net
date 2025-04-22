namespace antflowcore.constant.enums
{
    /// <summary>
    /// 常见错误枚举
    /// </summary>
    public enum CommonError
    {
        PARAM_BLANK = 1,            // 参数[%s]不能为空
        PARAMS_BLANK_SAME_TIME = 2, // 参数[%s],[%s]不能同时为空
        PARAM_INVALID = 3,          // 参数[%s]的值非法
        UN_KNOWN_ERROR = 4,         // 未知错误
        SERVER_EXCEPTION = 5,       // 服务端发生异常
        CONCURRENT_FAILURE = 6,     // 并发操作失败
        JSON_EXCEPTION = 7,         // JSON解析异常
        ENCODE_EXCEPTION = 8,       // Encode编码异常
        ACTION_NOT_SUPPORT = 9,     // 该操作不支持，原因：[%s]
        WITH_OUT_AUTHORIZATION = 10, // API调用无授权
        TIME_OUT = 11               // 超时[%s]
    }

    /// <summary>
    /// CommonError 的消息扩展
    /// </summary>
    public static class CommonErrorExtensions
    {
        public static string GetMsg(this CommonError error)
        {
            return error switch
            {
                CommonError.PARAM_BLANK => "参数[%s]不能为空",
                CommonError.PARAMS_BLANK_SAME_TIME => "参数[%s],[%s]不能同时为空",
                CommonError.PARAM_INVALID => "参数[%s]的值非法",
                CommonError.UN_KNOWN_ERROR => "未知错误",
                CommonError.SERVER_EXCEPTION => "服务端发生异常",
                CommonError.CONCURRENT_FAILURE => "并发操作失败",
                CommonError.JSON_EXCEPTION => "JSON解析异常",
                CommonError.ENCODE_EXCEPTION => "Encode编码异常",
                CommonError.ACTION_NOT_SUPPORT => "该操作不支持，原因：[%s]",
                CommonError.WITH_OUT_AUTHORIZATION => "API调用无授权",
                CommonError.TIME_OUT => "超时[%s]",
                _ => "未知错误"
            };
        }
    }
}