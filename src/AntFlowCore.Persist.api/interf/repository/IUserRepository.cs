using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserRepository : IBaseRepository<User>
{
    List<Department> QueryDepartmentAndUserByUserId(long userId);

    public List<User> QueryUserListByExpression(Expression<Func<User, bool>> expression,
        PagingInfo pagingInfo);
}
