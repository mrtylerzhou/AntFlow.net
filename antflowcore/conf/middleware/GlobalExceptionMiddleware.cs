using AntFlowCore.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace antflowcore.conf.middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // 继续处理请求
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex); // 捕获并处理异常
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string errorMsg = exception.InnerException!.Message??exception.Message;
        var response = Result<object>.NewFailureResult("500", errorMsg);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(response);
    }
}
