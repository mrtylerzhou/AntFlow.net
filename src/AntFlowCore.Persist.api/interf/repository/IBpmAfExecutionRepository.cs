using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmAfExecutionRepository: IBaseRepository<BpmAfExecution>
{
    public void DeleteByExpression(Expression<Func<BpmAfExecution, bool>> predicate);
    void UpdateTaskCount(string executionId, int taskCount);
}
