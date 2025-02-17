using Microsoft.AspNetCore.Http;

namespace antflowcore.aop;

public class TransactionalMiddleware
{
    private readonly RequestDelegate _next;

    public TransactionalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 设置服务提供程序
        TransactionalAttribute.SetServiceProvider(context.RequestServices);

        // 调用下一个中间件
        await _next(context);
    }
}
