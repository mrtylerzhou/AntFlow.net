using AntFlow.Core.Util;

namespace AntFlow.Core.Entity;

/// <summary>
///     ͨ�ý����?
/// </summary>
public class Result<T>
{
    //todo cbcbu
    private static readonly ILogger _logger = new Logger<string>(new LoggerFactory());

    /// <summary>
    ///     ���� ID
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    ///     ��Ӧ״̬��
    /// </summary>
    public int? Code { get; set; }

    /// <summary>
    ///     ��Ӧ����
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    ///     �����Ƿ�ɹ�?
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     �Ƿ���Ҫ����1
    /// </summary>
    public bool NeedRetry { get; set; }

    /// <summary>
    ///     �쳣��Ϣ
    /// </summary>
    public string ExpInfo { get; set; }

    /// <summary>
    ///     �������?
    /// </summary>
    public string ErrCode { get; set; }

    /// <summary>
    ///     ������Ϣ
    /// </summary>
    public string ErrMsg { get; set; }


    /// <summary>
    ///     �½�ʧ�ܵ� Result
    /// </summary>
    public static Result<object> NewFailureResult(CommonError error, bool needRetry, System.Exception exp)
    {
        Result<object>? result = new();
        result.ErrCode = error.GetMsg();
        result.ErrMsg = error.GetMsg();
        result.Success = false;
        result.ExpInfo = JiMuCommonUtils.ExceptionToString(exp);
        result.NeedRetry = needRetry;
        result.RequestId = MDCLogUtil.GetLogId(_logger);
        return result;
    }

    /// <summary>
    ///     �½�ʧ�ܵ� Result
    /// </summary>
    public static Result<object> NewFailureResult(string errCode, string errMsg)
    {
        Result<object>? result = new();
        result.ErrCode = errCode;
        result.ErrMsg = errMsg;
        result.Success = false;
        result.NeedRetry = false;
        result.RequestId = MDCLogUtil.GetLogId(_logger); //todo cbcbu
        return result;
    }

    /// <summary>
    ///     �½�ʧ�ܵ� Result
    /// </summary>
    public static Result<object> NewFailureResult(string errCode, string errMsg, bool needRetry, System.Exception exp)
    {
        Result<object>? result = new()
        {
            ErrCode = errCode,
            ErrMsg = errMsg,
            Success = false,
            NeedRetry = needRetry,
            RequestId = MDCLogUtil.GetLogId(_logger)
        };
        return result;
    }

    /// <summary>
    ///     �½��ɹ��� Result
    /// </summary>
    private static Result<T> NewSuccessResult(T data)
    {
        Result<T>? result = new() { Success = true, Data = data, Code = 200, RequestId = MDCLogUtil.GetLogId(_logger) };
        return result;
    }

    public static Result<T> Succ(T data)
    {
        return NewSuccessResult(data);
    }

    /// <summary>
    ///     �½��ɹ��� Result������Ϊ��
    /// </summary>
    public static Result<object> Succ()
    {
        return new Result<object>();
    }
}