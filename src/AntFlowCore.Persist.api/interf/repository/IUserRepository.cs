using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserRepository : IBaseRepository<User>
{
    List<Department> QueryDepartmentAndUserByUserId(long userId);

    /// <summary>
    /// 按用户查询其角色关联记录(t_user_role)
    /// </summary>
    List<UserRole> QueryUserRolesByUserId(long userId);

    public List<User> QueryUserListByExpression(Expression<Func<User, bool>> expression,
        PagingInfo pagingInfo);
}
