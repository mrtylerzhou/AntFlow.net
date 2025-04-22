using Microsoft.AspNetCore.Http;
using System.Text;

namespace antflowcore.http;

public static class HttpContextExtensions
{
    public static string ReadRawBodyAsString(this HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return body.Result;
    }
}