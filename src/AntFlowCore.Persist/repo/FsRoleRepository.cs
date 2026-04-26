using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsRoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public FsRoleRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BaseIdTranStruVo> QueryUserByRoleIds(ICollection<string> roleIds)
    {
        IEnumerable<long> roleIdsLong = AFCollectionUtil.StringToLongList(roleIds);
        List<BaseIdTranStruVo> users = _ormContext.FreeSql.Select<User, UserRole>()
            .InnerJoin((u, r) => u.Id == r.UserId)
            .Where((u, r) => roleIdsLong.Contains(r.RoleId ?? 0L))
            .ToList<BaseIdTranStruVo>(
                (a, b) => new BaseIdTranStruVo
                {
                    Id = a.Id.ToString(),
                    Name = a.Name,
                }
            );
        return users;
    }
}
