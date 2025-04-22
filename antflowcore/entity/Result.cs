using antflowcore.constant.enums;
using AntFlowCore.Util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// 通用结果类
    /// </summary>
    public class Result<T>
    {
        //todo cbcbu
        private static readonly ILogger _logger = new Logger<string>(new LoggerFactory());

        /// <summary>
        /// 请求 ID
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 是否需要重试1
        /// </summary>
        public bool NeedRetry { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExpInfo { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 新建失败的 Result
        /// </summary>
        public static Result<object> NewFailureResult(CommonError error, bool needRetry, Exception exp)
        {
            var result = new Result<object>();
            result.ErrCode = error.GetMsg();
            result.ErrMsg = error.GetMsg();
            result.Success = false;
            result.ExpInfo = JiMuCommonUtils.ExceptionToString(exp);
            result.NeedRetry = needRetry;
            result.RequestId = MDCLogUtil.GetLogId(_logger);
            return result;
        }

        /// <summary>
        /// 新建失败的 Result
        /// </summary>
        public static Result<object> NewFailureResult(string errCode, string errMsg)
        {
            var result = new Result<object>();
            result.ErrCode = errCode;
            result.ErrMsg = errMsg;
            result.Success = false;
            result.NeedRetry = false;
            result.RequestId = MDCLogUtil.GetLogId(_logger);//todo cbcbu
            return result;
        }

        /// <summary>
        /// 新建失败的 Result
        /// </summary>
        public static Result<object> NewFailureResult(string errCode, string errMsg, bool needRetry, Exception exp)
        {
            var result = new Result<object>();
            result.ErrCode = errCode;
            result.ErrMsg = errMsg;
            result.Success = false;
            result.NeedRetry = needRetry;
            result.RequestId = MDCLogUtil.GetLogId(logger: _logger);
            return result;
        }

        /// <summary>
        /// 新建成功的 Result
        /// </summary>
        private static Result<T> NewSuccessResult(T data)
        {
            var result = new Result<T>();
            result.Success = true;
            result.Data = data;
            result.Code = 200;
            result.RequestId = MDCLogUtil.GetLogId(logger: _logger);
            return result;
        }

        public static Result<T> Succ(T obj)
        {
            return NewSuccessResult(obj);
        }

        /// <summary>
        /// 新建成功的 Result，数据为空
        /// </summary>
        public static Result<object> Succ()
        {
            return new Result<object>();
        }
    }
}