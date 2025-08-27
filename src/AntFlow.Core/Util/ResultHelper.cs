using AntFlow.Core.Entity;

namespace AntFlow.Core.Util;

public static class ResultHelper
{
    private static readonly ILogger _logger = LoggerFactory.Create(builder => { }).CreateLogger("Result");

    public static Result<T> Success<T>(T data)
    {
        Result<T>? result = new() { Success = true, Data = data, Code = 200, RequestId = MDCLogUtil.GetLogId(_logger) };
        return result;
    }

    public static Result<T> Fail<T>(string errCode, string errMsg, bool needRetry, System.Exception exp)
    {
        Result<T>? result = new()
        {
            ErrCode = errCode,
            ErrMsg = errMsg,
            Success = false,
            NeedRetry = needRetry,
            RequestId = MDCLogUtil.GetLogId(_logger)
        };
        return result;
    }
}