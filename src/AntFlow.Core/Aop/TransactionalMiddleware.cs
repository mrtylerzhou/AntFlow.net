namespace AntFlow.Core.Aop;

public class TransactionalMiddleware
{
    private readonly RequestDelegate _next;

    public TransactionalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ���÷����ṩ����
        TransactionalAttribute.SetServiceProvider(context.RequestServices);

        // ������һ���м��
        await _next(context);
    }
}