using FreeSql;

namespace antflowcore.service.repository;

public abstract class AFBaseCurdRepositoryService<T> where T : class
{
    public readonly IFreeSql Frsql;
    public IBaseRepository<T> baseRepo{get;}
    public AFBaseCurdRepositoryService(IFreeSql freeSql)
    {
        Frsql = freeSql;
        baseRepo=freeSql.GetRepository<T>();
    }
}