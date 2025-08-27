using AntFlow.Core.Vo;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Util;

public class SecurityUtils
{
    // 获取当前登录员工信息
    public static BaseIdTranStruVo GetLogInEmpInfo()
    {
        return (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
    }

    // 获取当前登录员工 ID
    public static string GetLogInEmpId()
    {
        BaseIdTranStruVo? currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        if (currentuser == null)
        {
            throw new AntFlowException.AFBizException("用户未登录!");
        }

        return currentuser.Id;
    }

    // 获取当前登录员工 ID (如果为空返回 null 或 "-999")
    public static string GetLogInEmpIdStr()
    {
        BaseIdTranStruVo? currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        if (currentuser == null)
        {
            throw new AntFlowException.AFBizException("用户未登录!");
        }

        return currentuser.Id;
    }

    // 获取当前登录员工姓名
    public static string GetLogInEmpName()
    {
        BaseIdTranStruVo? currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        if (currentuser == null)
        {
            throw new AntFlowException.AFBizException("用户未登录!");
        }

        return currentuser.Name;
    }

    // 安全获取当前登录员工姓名，如果未登录返回默认值
    public static string GetLogInEmpNameSafe()
    {
        BaseIdTranStruVo? currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        if (currentuser == null)
        {
            return string.Empty;
        }

        return currentuser.Name;
    }

    // 安全获取当前登录员工 ID，如果未登录返回 "-999"
    public static string GetLogInEmpIdSafe()
    {
        BaseIdTranStruVo? currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        if (currentuser == null)
        {
            return "-999";
        }

        return currentuser.Id;
    }
}