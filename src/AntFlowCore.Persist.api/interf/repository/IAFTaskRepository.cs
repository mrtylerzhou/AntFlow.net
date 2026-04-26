using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFTaskRepository : IBaseRepository<BpmAfTask>
{
    public void DeleteByExpression(Expression<Func<BpmAfTask, bool>> predicate);
}