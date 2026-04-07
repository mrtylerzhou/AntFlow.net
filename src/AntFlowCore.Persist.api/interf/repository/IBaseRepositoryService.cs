using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBaseRepositoryService<T> where T : class
{
    IBaseRepository<T> baseRepo { get; }
    IFreeSql Frsql { get; }
}
