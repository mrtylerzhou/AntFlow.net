using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsUserRepository : RepositoryBase<User>, IUserRepository
{
    public SsUserRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<Department> QueryDepartmentAndUserByUserId(long userId)
    {
        return Db.Queryable<Department>()
            .InnerJoin<User>((a, b) => a.Id == b.DepartmentId)
            .Where((a, b) => b.Id == userId)
            .Select((a, b) => a)
            .ToList();
    }

    public List<User> QueryUserListByExpression(Expression<Func<User, bool>> expression, PagingInfo pagingInfo)
    {
        int totalCount = 0;
        List<User> users = Db.Queryable<User>()
            .Where(expression)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return users;
    }
}
