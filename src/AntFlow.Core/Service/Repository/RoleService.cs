using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;

namespace AntFlow.Core.Service.Repository;

public class RoleService : AFBaseCurdRepositoryService<Role>, IRoleService
{
    public RoleService(IFreeSql freeSql) : base(freeSql)
    {
    }

    /// <summary>
    ///     ???User??????¦Ç????,????????Id??Name???,??????????????????????????
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public List<User> QueryUserByRoleIds(ICollection<string> roleIds)
    {
        IEnumerable<long> roleIdsLong = AFCollectionUtil.StringToLongList(roleIds);
        List<User> users = Frsql.Select<User, UserRole>()
            .InnerJoin((u, r) => u.Id == r.UserId)
            .Where((u, r) => roleIdsLong.Contains(r.RoleId ?? 0L)).ToList();
        return users;
    }
}