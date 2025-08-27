using FreeSql;

namespace AntFlow.Core.Service.Repository;

public abstract class AFBaseCurdRepositoryService<T> where T : class
{
    public readonly IFreeSql Frsql;

    public AFBaseCurdRepositoryService(IFreeSql freeSql)
    {
        Frsql = freeSql;
        baseRepo = freeSql.GetRepository<T>();
    }

    public IBaseRepository<T> baseRepo { get; }
}