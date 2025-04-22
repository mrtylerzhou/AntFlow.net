using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Util;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace antflowcore.conf.middleware;

public class HeaderMiddleware
{
    private readonly RequestDelegate _next;

    public HeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserService userService)
    {
        if (context.Request.Method != HttpMethod.Options.Method)
        {
        }
        if (context.Request.Headers.TryGetValue("userId", out var userIdValue))
        {
            context.Request.Headers.TryGetValue("userName", out var userNameValue);
            string decodedUserName = WebUtility.UrlDecode(userNameValue.ToString());
            string userId = userIdValue.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                context.Request.Headers.TryGetValue("Userid", out var userIdValue1);
                userId = userIdValue1.ToString();
            }

            if (!string.IsNullOrEmpty(userId))
            {
                if (!string.IsNullOrEmpty(decodedUserName))
                {
                    BaseIdTranStruVo userInfo = new BaseIdTranStruVo
                    {
                        Id = userId,
                        Name = decodedUserName,
                    };
                    ThreadLocalContainer.Set("currentuser", userInfo);
                }
                else
                {
                    BaseIdTranStruVo struVo = userService.GetById(userId);
                    ThreadLocalContainer.Set("currentuser", struVo);
                }
            }
        }

        await _next(context);
    }
}