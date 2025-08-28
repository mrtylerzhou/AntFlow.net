using System.Text;

namespace AntFlow.Core.Http;

public static class HttpContextExtensions
{
    public static string ReadRawBodyAsString(this HttpContext context)
    {
        context.Request.EnableBuffering();
        using StreamReader? reader = new(context.Request.Body, Encoding.UTF8);
        Task<string>? body = reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return body.Result;
    }
}