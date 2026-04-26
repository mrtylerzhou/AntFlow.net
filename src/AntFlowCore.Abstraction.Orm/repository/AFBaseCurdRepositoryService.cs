
using FreeSql;

namespace AntFlowCore.Abstraction.Orm.repository;

public abstract class AFBaseCurdRepositoryService<T> where T : class
{
    public IFreeSql Frsql { get; }
    public IBaseRepository<T> baseRepo{get;set;}
    public AFBaseCurdRepositoryService(IFreeSql freeSql)
    {
        Frsql = freeSql;
        baseRepo=freeSql.GetRepository<T>();
    }
}