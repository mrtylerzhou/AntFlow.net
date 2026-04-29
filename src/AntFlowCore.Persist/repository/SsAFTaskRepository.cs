using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsAFTaskRepository : RepositoryBase<BpmAfTask>, IAFTaskRepository
{
    public SsAFTaskRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
    
    public void DeleteByExpression(Expression<Func<BpmAfTask, bool>> predicate)
    {
        Db.Deleteable<BpmAfTask>()
            .Where(predicate)
            .ExecuteCommand();
    }

    public int UpdateAssignee(string taskId, string assignee, string assigneeName)
    {
        int affrows = Db.Updateable<BpmAfTask>()
            .SetColumns(a => a.Assignee == assignee)
            .SetColumns(a => a.AssigneeName == assigneeName)
            .Where(a => a.Id == taskId)
            .ExecuteCommand();
        return affrows;
    }

    public List<BpmAfTask> FindTasksByProcessNumber(string processNumber)
    {
        return Db.Queryable<BpmAfTask>()
            .InnerJoin<BpmBusinessProcess>((a, b) => a.ProcInstId == b.ProcInstId)
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList();
    }
}
