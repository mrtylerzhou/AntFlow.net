using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace antflow.abp.Controllers
{
    public abstract class AntflowControllerBase : AbpController
    {
        protected AntflowControllerBase()
        {
            LocalizationSourceName = flowerConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
