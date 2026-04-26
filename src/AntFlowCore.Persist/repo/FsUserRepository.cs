using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repo;

public class FsUserRepository : RepositoryBase<User>, IUserRepository
{
    public FsUserRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<Department> QueryDepartmentAndUserByUserId(long userId)
    {
        return _ormContext.FreeSql.Select<Department, User>()
            .InnerJoin((a, b) => a.Id == b.DepartmentId)
            .Where((a, b) => b.Id == userId)
            .ToList();
    }
    
    public List<User> QueryUserListByExpression( Expression<Func<User, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<User> users = _ormContext.FreeSql.GetRepository<User>()
            .Where(expression)
            .Page(basePagingInfo)
            .ToList();
        pagingInfo.Count = users.Count;
        pagingInfo.PageNumber = basePagingInfo.PageNumber;
        pagingInfo.PageSize = basePagingInfo.PageSize;
        return users;
    }
}
