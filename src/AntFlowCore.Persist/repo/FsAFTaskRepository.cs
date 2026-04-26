using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

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
}