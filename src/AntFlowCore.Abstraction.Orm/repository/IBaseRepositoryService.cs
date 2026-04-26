using FreeSql;

namespace AntFlowCore.Abstraction.Orm.repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseRepositoryService<T> where T : class
{
    IBaseRepository<T> baseRepo { get; }
    IFreeSql Frsql { get; }
}
