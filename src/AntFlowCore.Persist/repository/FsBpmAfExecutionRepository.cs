using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repositorysitory;

public class FsBpmAfExecutionRepository : RepositoryBase<BpmAfExecution>, IBpmAfExecutionRepository
{
    public FsBpmAfExecutionRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
    
    public void DeleteByExpression(Expression<Func<BpmAfExecution, bool>> predicate)
    {
        _ormContext.FreeSql.Delete<BpmAfExecution>()
            .Where(predicate)
            .ExecuteAffrows();
    }

    public void UpdateTaskCount(string executionId, int taskCount)
    {
        _ormContext.FreeSql.Update<BpmAfExecution>()
            .Set(a => a.TaskCount == taskCount)
            .Where(a => a.Id == executionId)
            .ExecuteAffrows();
    }
}