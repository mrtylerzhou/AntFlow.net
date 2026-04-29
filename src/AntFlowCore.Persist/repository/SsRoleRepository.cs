using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsRoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public SsRoleRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds)
    {
        IEnumerable<long> roleIdsLong = AFCollectionUtil.StringToLongList(roleIds);
        List<BaseIdTranStruVo> users = Db.Queryable<User>()
            .InnerJoin<UserRole>((u, r) => u.Id == r.UserId)
            .Where((u, r) => roleIdsLong.Contains(r.RoleId ?? 0L))
            .Select((u, r) => new BaseIdTranStruVo
            {
                Id = u.Id.ToString(),
                Name = u.Name,
            })
            .ToList();
        return users;
    }
}
