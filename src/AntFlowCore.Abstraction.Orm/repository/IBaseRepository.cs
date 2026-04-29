using System.Linq.Expressions;
using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.repository;

public interface IBaseRepository<TEntity> where TEntity : class
{
    ISugarQueryable<TEntity> GetQueryable();
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
