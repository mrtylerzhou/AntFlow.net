using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repositorysitory;

public class FsAFTaskRepository: RepositoryBase<BpmAfTask>, IAFTaskRepository
{
    public FsAFTaskRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
    
    public void DeleteByExpression(Expression<Func<BpmAfTask, bool>> predicate)
    {
        _ormContext.FreeSql.Delete<BpmAfTask>()
            .Where(predicate)
            .ExecuteAffrows();
    }

    public int UpdateAssignee(string taskId, string assignee, string assigneeName)
    {
        int executeAffrows = _ormContext.FreeSql.Update<BpmAfTask>()
            .Set(a => a.Assignee, assignee)
            .Set(a => a.AssigneeName, assigneeName)
            .Where(a => a.Id == taskId)
            .ExecuteAffrows();
        return executeAffrows;
    }

    public List<BpmAfTask> FindTasksByProcessNumber(string processNumber)
    {
        return _ormContext.FreeSql
            .Select<BpmAfTask>()
            .From<BpmBusinessProcess>((a, b) =>
                a.InnerJoin(x => x.ProcInstId == b.ProcInstId)
            )
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList();
    }
}