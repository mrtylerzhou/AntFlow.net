using AntFlow.Core.Constant;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace AntFlow.Core.Configuration.Middleware;

public class HeaderMiddleware
{
    private readonly RequestDelegate _next;

    public HeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserService userService, ITenantIdHolder tenantIdHolder)
    {
        if (context.Request.Method != HttpMethod.Options.Method)
        {
            if (context.Request.Headers.TryGetValue("userId", out StringValues userIdValue))
            {
                context.Request.Headers.TryGetValue("userName", out StringValues userNameValue);
                string decodedUserName = WebUtility.UrlDecode(userNameValue.ToString());
                string userId = userIdValue.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    context.Request.Headers.TryGetValue("Userid", out StringValues userIdValue1);
                    userId = userIdValue1.ToString();
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    if (!string.IsNullOrEmpty(decodedUserName))
                    {
                        BaseIdTranStruVo userInfo = new() { Id = userId, Name = decodedUserName };
                        ThreadLocalContainer.Set("currentuser", userInfo);
                    }
                    else
                    {
                        BaseIdTranStruVo struVo = userService.GetById(userId);
                        ThreadLocalContainer.Set("currentuser", struVo);
                    }
                }
            }

            if (context.Request.Headers.TryGetValue(StringConstants.TENANT_ID, out StringValues tenantId))
            {
                tenantIdHolder.SetCurrentTenantId(tenantId);
            }
        }

        context.Response.OnCompleted(() =>
        {
            ThreadLocalContainer.Clean();
            return Task.CompletedTask;
        });
        await _next(context);
    }
}