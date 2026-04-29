using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmAfExecutionRepository : RepositoryBase<BpmAfExecution>, IBpmAfExecutionRepository
{
    public SsBpmAfExecutionRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
    
    public void DeleteByExpression(Expression<Func<BpmAfExecution, bool>> predicate)
    {
        Db.Deleteable<BpmAfExecution>()
            .Where(predicate)
            .ExecuteCommand();
    }

    public void UpdateTaskCount(string executionId, int taskCount)
    {
        Db.Updateable<BpmAfExecution>()
            .SetColumns(a => a.TaskCount == taskCount)
            .Where(a => a.Id == executionId)
            .ExecuteCommand();
    }
}
