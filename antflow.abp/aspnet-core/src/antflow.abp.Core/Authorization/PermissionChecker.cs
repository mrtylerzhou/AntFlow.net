using Abp.Authorization;
using antflow.abp.Authorization.Roles;
using antflow.abp.Authorization.Users;


namespace antflow.abp.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
