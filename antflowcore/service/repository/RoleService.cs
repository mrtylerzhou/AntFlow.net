using System.Collections.ObjectModel;
using AntFlowCore.Entities;
using antflowcore.entity;
using antflowcore.util;

namespace antflowcore.service.repository;

public class RoleService: AFBaseCurdRepositoryService<Role>
{
    public RoleService(IFreeSql freeSql) : base(freeSql)
    {
    }

    /// <summary>
    /// 虽然User对象字段非常多,但是只需要Id和Name字段,如果有特殊业务可以选择出更多字段
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public List<User> QueryUserByRoleIds(ICollection<String> roleIds)
    {
        IEnumerable<long> roleIdsLong = AFCollectionUtil.StringToLongList(roleIds);
        List<User> users = Frsql.Select<User, UserRole>()
            .InnerJoin((u, r) => u.Id == r.UserId)
            .Where((u, r) => roleIdsLong.Contains(r.RoleId ?? 0L)).ToList();
        return users;
    }
}