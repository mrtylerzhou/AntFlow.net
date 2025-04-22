using AntFlowCore.Util;

namespace antflowcore.util;

using AntFlowCore.Entity;
using Microsoft.Extensions.Logging;

public static class ResultHelper
{
    private static readonly ILogger _logger = LoggerFactory.Create(builder => { }).CreateLogger("Result");

    public static Result<T> Success<T>(T data)
    {
        var result = new Result<T>
        {
            Success = true,
            Data = data,
            Code = 200,
            RequestId = MDCLogUtil.GetLogId(logger: _logger)
        };
        return result;
    }

    public static Result<T> Fail<T>(string errCode, string errMsg, bool needRetry, Exception exp)
    {
        var result = new Result<T>
        {
            ErrCode = errCode,
            ErrMsg = errMsg,
            Success = false,
            NeedRetry = needRetry,
            RequestId = MDCLogUtil.GetLogId(logger: _logger)
        };
        return result;
    }
}