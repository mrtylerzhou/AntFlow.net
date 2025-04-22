using antflowcore.exception;
using AntFlowCore.Util;
using antflowcore.vo;

namespace antflowcore.util
{
    public class SecurityUtils
    {
        // 获取当前登录用户的全部信息
        public static BaseIdTranStruVo GetLogInEmpInfo()
        {
            return (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
        }

        // 获取当前登录用户的 ID
        public static string GetLogInEmpId()
        {
            var currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
            if (currentuser == null)
            {
                throw new AFBizException("当前用户未登陆!");
            }
            return currentuser.Id;
        }

        // 获取当前登录用户的 ID (可以返回 null 或 "-999")
        public static string GetLogInEmpIdStr()
        {
           
            var currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
            if (currentuser == null)
            {
                throw new AFBizException("当前用户未登陆!");
            }
            return currentuser.Id;
        }

        // 获取当前登录用户的姓名
        public static string GetLogInEmpName()
        {
           
            var currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
            if (currentuser == null)
            {
                throw new AFBizException("当前用户未登陆!");
            }
            return currentuser.Name;
        }

        // 安全地获取当前登录用户的姓名，如果未登录则返回空字符串
        public static string GetLogInEmpNameSafe()
        {
           
            var currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
            if (currentuser == null)
            {
                return string.Empty;
            }
            return currentuser.Name;
        }

        // 安全地获取当前登录用户的 ID，如果未登录则返回 "-999"
        public static string GetLogInEmpIdSafe()
        {
          
            var currentuser = (BaseIdTranStruVo)ThreadLocalContainer.Get("currentuser");
            if (currentuser == null)
            {
                return "-999";
            }
            return currentuser.Id;
        }
    }
    
}
