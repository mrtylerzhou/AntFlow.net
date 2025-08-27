using AntFlow.Core.Entity;

namespace AntFlow.Core.Configuration.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // ������������
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex); // ���񲢴����쳣
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, System.Exception exception)
    {
        string errorMsg = exception.InnerException?.Message ?? exception.Message;
        Result<object>? response = Result<object>.NewFailureResult("200", errorMsg);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status200OK;

        return context.Response.WriteAsJsonAsync(response);
    }
}