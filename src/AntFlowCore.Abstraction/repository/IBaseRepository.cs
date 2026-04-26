using System.Linq.Expressions;

namespace AntFlowCore.Abstraction.repository;

/// <summary>
/// 基础仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IBaseRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetQueryable();
    TEntity? GetById(object id);
    List<TEntity> GetAll();
    List<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    TEntity? FirstOrDefault();
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    int Count(Expression<Func<TEntity, bool>>? predicate = null);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    int SaveChanges();
}
